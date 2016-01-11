using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;

namespace EfCreateObj
{
    public class SoftDeleteInterceptor : IDbCommandTreeInterceptor
    {
        public void TreeCreated(DbCommandTreeInterceptionContext interceptionContext)
        {
            if (interceptionContext.OriginalResult.DataSpace == DataSpace.SSpace)
            {
                var queryCommand = interceptionContext.Result as DbQueryCommandTree;
                if (queryCommand != null)
                {
                    var newQuery = queryCommand.Query.Accept(new SoftDeleteQueryVisitor());
                    interceptionContext.Result = new DbQueryCommandTree(
                        queryCommand.MetadataWorkspace,
                        queryCommand.DataSpace,
                        newQuery);
                }

                var deleteCommand = interceptionContext.OriginalResult as DbDeleteCommandTree;
                if (deleteCommand != null)
                {
                    //Check if POCO has system Status flag
                    if (
                        ((EntityType)deleteCommand.Target.VariableType.EdmType).DeclaredMembers.All(
                            x => x.Name != "Status"))
                        return;

                    // Just because the entity has the soft delete annotation doesn't mean that 
                    // this particular table has the column. This occurs in situation like TPT
                    // inheritance mapping and entity splitting where one type maps to multiple 
                    // tables.
                    // If the table doesn't have the column we just want to leave the row unchanged
                    // since it will be joined to the table that does have the column during query.
                    // We can't no-op, so we just generate an UPDATE command that doesn't set anything.
                    var setClauses = new List<DbModificationClause>();
                    var table = (EntityType)deleteCommand.Target.VariableType.EdmType;
                    if (table.Properties.Any(p => p.Name == "Status"))
                    {
                        setClauses.Add(DbExpressionBuilder.SetClause(
                            deleteCommand.Target.VariableType.Variable(deleteCommand.Target.VariableName)
                                         .Property("Status"),
                            DbExpression.FromInt32((int)SystemStatus.Deleted)));
                    }

                    var update = new DbUpdateCommandTree(
                        deleteCommand.MetadataWorkspace,
                        deleteCommand.DataSpace,
                        deleteCommand.Target,
                        deleteCommand.Predicate,
                        setClauses.AsReadOnly(),
                        null);

                    interceptionContext.Result = update;
                }
            }
        }
    }
}
