using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;

namespace Immortals
{
    class IngredientValueGetter_Immortality : IngredientValueGetter
    {
        public override float ValuePerUnitOf(ThingDef t)
        {
            return 0;
        }

        public override string BillRequirementsDescription(RecipeDef r, IngredientCount ing)
        {
            throw new NotImplementedException();
        }
    }
}
