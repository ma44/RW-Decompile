using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobDriver_Vomit : JobDriver
	{
		private int ticksLeft;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.ticksLeft, "ticksLeft", 0, false);
		}

		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			Toil to = new Toil();
			to.initAction = delegate
			{
				this.<>f__this.ticksLeft = Rand.Range(300, 900);
				int num = 0;
				IntVec3 intVec;
				while (true)
				{
					intVec = this.<>f__this.pawn.Position + GenAdj.AdjacentCellsAndInside[Rand.Range(0, 9)];
					num++;
					if (num > 12)
					{
						break;
					}
					if (intVec.InBounds() && intVec.Standable())
					{
						goto IL_81;
					}
				}
				intVec = this.<>f__this.pawn.Position;
				IL_81:
				this.<>f__this.pawn.CurJob.targetA = intVec;
				this.<>f__this.pawn.Drawer.rotator.FaceCell(intVec);
				this.<>f__this.pawn.pather.StopDead();
			};
			to.tickAction = delegate
			{
				if (this.<>f__this.ticksLeft % 150 == 149)
				{
					FilthMaker.MakeFilth(this.<>f__this.pawn.CurJob.targetA.Cell, ThingDefOf.FilthVomit, this.<>f__this.pawn.LabelIndefinite(), 1);
					if (this.<>f__this.pawn.needs.food.CurLevelPercentage > 0.1f)
					{
						this.<>f__this.pawn.needs.food.CurLevel -= this.<>f__this.pawn.needs.food.MaxLevel * 0.04f;
					}
				}
				this.<>f__this.ticksLeft--;
				if (this.<>f__this.ticksLeft <= 0)
				{
					this.<>f__this.ReadyForNextToil();
					TaleRecorder.RecordTale(TaleDefOf.Vomited, new object[]
					{
						this.<>f__this.pawn
					});
				}
			};
			to.defaultCompleteMode = ToilCompleteMode.Never;
			to.WithEffect("Vomit", TargetIndex.A);
			to.PlaySustainerOrSound(() => SoundDef.Named("Vomit"));
			yield return to;
		}
	}
}