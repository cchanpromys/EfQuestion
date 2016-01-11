using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace EfCreateObj
{
    public class SoftDeleteQueryVisitor : DefaultExpressionVisitor
    {
        public override DbExpression Visit(DbScanExpression expression)
        {
            //Check if POCO has system Status flag
            if (expression.Target.ElementType.Members.All(x => x.Name != "SystemStatus"))
                return base.Visit(expression);

            //**** Copied this from the EF team: https://github.com/rowanmiller/Demo-TechEd2014 ****

            // Just because the entity has the soft delete annotation doesn't mean that 
            // this particular table has the column. This occurs in situation like TPT
            // inheritance mapping and entity splitting where one type maps to multiple 
            // tables.
            // We only apply the filter if the column is actually present in this table.
            // If not, then the query is going to be joining to the table that does have
            // the column anyway, so the filter will still be applied.
            var table = (EntityType)expression.Target.ElementType;
            if (table.Properties.Any(p => p.Name == "Status"))
            {
                var binding = expression.Bind();
                return
                    binding.Filter(
                        binding.VariableType.Variable(binding.VariableName)
                               .Property("Status")
                               .NotEqual(DbExpression.FromInt32((int)SystemStatus.Deleted)));
            }

            return base.Visit(expression);
        }
    }
}
