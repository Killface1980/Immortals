using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace Immortals
{
    // Token: 0x020010B8 RID: 4280
    [StaticConstructorOnStartup]
    public class Building_ImmortalExtractor : Building
    {
        const int fillMax = 10;
        const float tickProgress = 2.7777778E-06f;
        //const float tickProgress = 2.7777778E-06f;
        const float tierMult = 0.5f;

        public float Progress
        {
            get
            {
                return this.progressInt;
            }
            set
            {
                if (value == this.progressInt)
                {
                    return;
                }
                this.progressInt = value;
                this.barFilledCachedMat = null;
            }
        }

        // Token: 0x170011A6 RID: 4518
        // (get) Token: 0x060066B7 RID: 26295 RVA: 0x0022E338 File Offset: 0x0022C538
        private Material BarFilledMat
        {
            get
            {
                if (this.barFilledCachedMat == null)
                {
                    this.barFilledCachedMat = SolidColorMaterials.SimpleSolidColorMaterial(Color.Lerp(Building_ImmortalExtractor.BarZeroProgressColor, Building_ImmortalExtractor.BarFermentedColor, this.Progress), false);
                }
                return this.barFilledCachedMat;
            }
        }

        // Token: 0x170011A7 RID: 4519
        // (get) Token: 0x060066B8 RID: 26296 RVA: 0x0022E36F File Offset: 0x0022C56F
        public int SpaceLeft
        {
            get
            {
                if (this.Finished)
                {
                    return 0;
                }
                return fillMax - this.fillCount;
            }
        }

        // Token: 0x170011A8 RID: 4520
        // (get) Token: 0x060066B9 RID: 26297 RVA: 0x0022E384 File Offset: 0x0022C584
        private bool Empty
        {
            get
            {
                return this.fillCount <= 0;
            }
        }

        // Token: 0x170011A9 RID: 4521
        // (get) Token: 0x060066BA RID: 26298 RVA: 0x0022E392 File Offset: 0x0022C592
        public bool Finished
        {
            get
            {
                return !this.Empty && this.Progress >= 1f;
            }
        }
        public bool Quickened
        {
            get
            {
                return this.hasQuickened;
            }
        }

        // Token: 0x170011AA RID: 4522
        // (get) Token: 0x060066BB RID: 26299 RVA: 0x0022E3B0 File Offset: 0x0022C5B0
        private float CurrentTempProgressSpeedFactor
        {
            get
            {
                float ambientTemperature = base.AmbientTemperature;

                if (ambientTemperature <= 0)
                {
                    return 0;
                }

                if (ambientTemperature <= 32)
                {
                    return Mathf.Lerp(0.5f, 1, ambientTemperature / 32f);
                }

                if (ambientTemperature <= 100)
                {
                    return Mathf.Lerp(1, 5, ambientTemperature / 100f);
                }

                return 5;
            }
        }

        // Token: 0x170011AB RID: 4523
        // (get) Token: 0x060066BC RID: 26300 RVA: 0x0022E408 File Offset: 0x0022C608
        private float ProgressPerTickAtCurrentTemp
        {
            get
            {
                return tickProgress * this.CurrentTempProgressSpeedFactor;
            }
        }

        public void Quicken(float value)
        {
            this.quickenMult = value;
            this.hasQuickened = true;
        }

        // Token: 0x170011AC RID: 4524
        // (get) Token: 0x060066BD RID: 26301 RVA: 0x0022E416 File Offset: 0x0022C616
        private int EstimatedTicksLeft
        {
            get
            {
                return Mathf.Max(Mathf.RoundToInt((1f - this.Progress) / this.ProgressPerTickAtCurrentTemp), 0);
            }
        }

        // Token: 0x060066BE RID: 26302 RVA: 0x0022E436 File Offset: 0x0022C636
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.fillCount, "fillCount", 0, false);
            Scribe_Values.Look<int>(ref this.outputCount, "outPutCount", 0, false);
            Scribe_Values.Look<float>(ref this.progressInt, "progress", 0f, false);
        }

        // Token: 0x060066BF RID: 26303 RVA: 0x0022E466 File Offset: 0x0022C666
        public override void TickRare()
        {
            base.TickRare();
            if (!this.Empty && this.Quickened)
            {
                this.Progress = Mathf.Min(this.Progress + 250f * this.ProgressPerTickAtCurrentTemp, 1f);
            }
        }

        // Token: 0x060066C0 RID: 26304 RVA: 0x0022E49C File Offset: 0x0022C69C
        public void AddParts(int count)
        {
            if (this.Finished)
            {
                Log.Warning("Tried to add heads to a finished batch.");
                return;
            }
            int num = Mathf.Min(count, fillMax - this.fillCount);
            if (num <= 0)
            {
                return;
            }
            this.Progress = GenMath.WeightedAverage(0f, (float)num, this.Progress, (float)this.fillCount);
            this.fillCount += num;
        }

        private void Reset()
        {
            this.fillCount = 0;
            this.Progress = 0f;
            this.hasQuickened = false;
            this.outputCount = 0;
        }
        public void AddPart(Thing part)
        {
            int num = Mathf.Min(part.stackCount, fillMax - this.fillCount);
            if (num <= 0)
            {
                return;
            }

            ThingWithComps partWithComps;
            if (part is ThingWithComps)
            {
                partWithComps = part as ThingWithComps;
                if (part.def == DefDatabase<ThingDef>.GetNamed("IH_Head"))
                {
                    foreach (ThingComp comp in partWithComps.AllComps)
                    {
                        if (comp is Immortal_HeadComp)
                        {
                            Immortal_HeadComp headComp = comp as Immortal_HeadComp;
                            this.outputCount += num * Mathf.RoundToInt(Math.Max(headComp.ImmortalLevel * headComp.ImmortalLevel * tierMult, 1f));
                        }
                    }
                    part.SplitOff(num).Destroy(DestroyMode.Vanish);
                    this.AddParts(num);
                }
            }
        }

        // Token: 0x060066C4 RID: 26308 RVA: 0x0022E570 File Offset: 0x0022C770
        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append(base.GetInspectString());
            if (stringBuilder.Length != 0)
            {
                stringBuilder.AppendLine();
            }
            if (!this.Empty && !this.hasQuickened)
            {
                stringBuilder.AppendLine("IH_Extractor_Contained_Heads".Translate(this.fillCount, fillMax));
                if (!this.Empty)
                {
                    stringBuilder.AppendLine("IH_Extractor_Awaiting".Translate());
                }
            }
            else if (this.hasQuickened)
            {
                if (this.Finished)
                {
                    stringBuilder.AppendLine("IH_Extractor_Finished".Translate());
                }
                else
                {
                    stringBuilder.AppendLine("IH_Extractor_Progress".Translate(this.Progress.ToStringPercent(), this.EstimatedTicksLeft.ToStringTicksToPeriod(true, false, true, true)));
                }
            }
            stringBuilder.AppendLine("Temperature".Translate() + ": " + base.AmbientTemperature.ToStringTemperature("F0"));
            stringBuilder.AppendLine("IH_Extractor_Speed_Bonus".Translate() + ": " + (this.CurrentTempProgressSpeedFactor - 1).ToString("0.0"));
            return stringBuilder.ToString().TrimEndNewlines();
        }

        // Token: 0x060066C5 RID: 26309 RVA: 0x0022E740 File Offset: 0x0022C940
        public Thing TakeOutImmortalis()
        {
            if (!this.Finished)
            {
                Log.Warning("Tried to get beer but it's not yet fermented.");
                return null;
            }
            Thing thing = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("IH_Mortalis"), null);
            thing.stackCount = Mathf.Clamp(Mathf.RoundToInt(this.outputCount * this.quickenMult), 10, 100000);
            this.Reset();
            return thing;
        }

        // Token: 0x060066C6 RID: 26310 RVA: 0x0022E774 File Offset: 0x0022C974
        public override void Draw()
        {
            base.Draw();
            if (!this.Empty)
            {
                Vector3 drawPos = this.DrawPos;
                drawPos.y += 0.04054054f;
                drawPos.z += 0.25f;
                GenDraw.DrawFillableBar(new GenDraw.FillableBarRequest
                {
                    center = drawPos,
                    size = Building_ImmortalExtractor.BarSize,
                    fillPercent = (float)this.fillCount / fillMax,
                    filledMat = this.BarFilledMat,
                    unfilledMat = Building_ImmortalExtractor.BarUnfilledMat,
                    margin = 0.1f,
                    rotation = Rot4.North
                });
            }
        }

        // Token: 0x060066C7 RID: 26311 RVA: 0x0022E820 File Offset: 0x0022CA20
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            IEnumerator<Gizmo> enumerator = null;
            if (Prefs.DevMode)
            {
                if (!this.Empty)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "DEV: Set progress to 1",
                        action = delegate ()
                        {
                            this.Progress = 1f;
                        }
                    };
                }
                if (this.SpaceLeft > 0)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "DEV: Fill",
                        action = delegate ()
                        {
                            this.Progress = 1f;
                            this.fillCount = fillMax;
                        }
                    };
                }
            }
            yield break;
        }

        // Token: 0x040039B8 RID: 14776
        private int fillCount;
        private int outputCount;
        private float quickenMult;

        // Token: 0x040039B9 RID: 14777
        private float progressInt;
        private bool hasQuickened;

        // Token: 0x040039BA RID: 14778
        private Material barFilledCachedMat;


        // Token: 0x040039BC RID: 14780
        private const int BaseFermentationDuration = 360000;


        // Token: 0x040039BE RID: 14782
        private static readonly Vector2 BarSize = new(0.55f, 0.1f);

        // Token: 0x040039BF RID: 14783
        private static readonly Color BarZeroProgressColor = new(0.4f, 0.27f, 0.22f);

        // Token: 0x040039C0 RID: 14784
        private static readonly Color BarFermentedColor = new(0.9f, 0.85f, 0.2f);

        // Token: 0x040039C1 RID: 14785
        private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);
    }
}
