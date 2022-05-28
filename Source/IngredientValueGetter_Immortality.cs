using System;

using Verse;

namespace Immortals
{
    public class IngredientValueGetter_Immortality : IngredientValueGetter
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
