using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Verse
{
	public class RecipeDef : Def
	{
		public Type workerClass = typeof(RecipeWorker);

		public Type workerCounterClass = typeof(RecipeWorkerCounter);

		[MustTranslate]
		public string jobString = "Doing an unknown recipe.";

		public WorkTypeDef requiredGiverWorkType;

		public float workAmount = -1f;

		public StatDef workSpeedStat;

		public StatDef efficiencyStat;

		public List<IngredientCount> ingredients = new List<IngredientCount>();

		public ThingFilter fixedIngredientFilter = new ThingFilter();

		public ThingFilter defaultIngredientFilter;

		public bool allowMixingIngredients;

		private Type ingredientValueGetterClass = typeof(IngredientValueGetter_Volume);

		public List<SpecialThingFilterDef> forceHiddenSpecialFilters;

		public List<ThingCountClass> products = new List<ThingCountClass>();

		public List<SpecialProductType> specialProducts;

		public bool productHasIngredientStuff;

		public int targetCountAdjustment = 1;

		public ThingDef unfinishedThingDef;

		public List<SkillRequirement> skillRequirements;

		public SkillDef workSkill;

		public float workSkillLearnFactor = 1f;

		public EffecterDef effectWorking;

		public SoundDef soundWorking;

		public List<ThingDef> recipeUsers;

		public List<BodyPartDef> appliedOnFixedBodyParts = new List<BodyPartDef>();

		public HediffDef addsHediff;

		public HediffDef removesHediff;

		public bool hideBodyPartNames;

		public bool isViolation;

		[MustTranslate]
		public string successfullyRemovedHediffMessage;

		public float surgerySuccessChanceFactor = 1f;

		public float deathOnFailedSurgeryChance;

		public bool targetsBodyPart = true;

		public bool anesthetize = true;

		public ResearchProjectDef researchPrerequisite;

		public ConceptDef conceptLearned;

		[Unsaved]
		private RecipeWorker workerInt;

		[Unsaved]
		private RecipeWorkerCounter workerCounterInt;

		[Unsaved]
		private IngredientValueGetter ingredientValueGetterInt;

		[Unsaved]
		private List<ThingDef> premultipliedSmallIngredients;

		public RecipeWorker Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (RecipeWorker)Activator.CreateInstance(this.workerClass);
					this.workerInt.recipe = this;
				}
				return this.workerInt;
			}
		}

		public RecipeWorkerCounter WorkerCounter
		{
			get
			{
				if (this.workerCounterInt == null)
				{
					this.workerCounterInt = (RecipeWorkerCounter)Activator.CreateInstance(this.workerCounterClass);
					this.workerCounterInt.recipe = this;
				}
				return this.workerCounterInt;
			}
		}

		public IngredientValueGetter IngredientValueGetter
		{
			get
			{
				if (this.ingredientValueGetterInt == null)
				{
					this.ingredientValueGetterInt = (IngredientValueGetter)Activator.CreateInstance(this.ingredientValueGetterClass);
				}
				return this.ingredientValueGetterInt;
			}
		}

		public bool AvailableNow
		{
			get
			{
				return this.researchPrerequisite == null || this.researchPrerequisite.IsFinished;
			}
		}

		public string MinSkillString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = false;
				if (this.skillRequirements != null)
				{
					for (int i = 0; i < this.skillRequirements.Count; i++)
					{
						SkillRequirement skillRequirement = this.skillRequirements[i];
						stringBuilder.AppendLine(string.Concat(new object[]
						{
							"   ",
							skillRequirement.skill.skillLabel,
							": ",
							skillRequirement.minLevel
						}));
						flag = true;
					}
				}
				if (!flag)
				{
					stringBuilder.AppendLine("   (" + "NoneLower".Translate() + ")");
				}
				return stringBuilder.ToString();
			}
		}

		public IEnumerable<ThingDef> AllRecipeUsers
		{
			get
			{
				RecipeDef.<>c__Iterator1D6 <>c__Iterator1D = new RecipeDef.<>c__Iterator1D6();
				<>c__Iterator1D.<>f__this = this;
				RecipeDef.<>c__Iterator1D6 expr_0E = <>c__Iterator1D;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}

		public bool UsesUnfinishedThing
		{
			get
			{
				return this.unfinishedThingDef != null;
			}
		}

		public float WorkAmountTotal(ThingDef stuffDef)
		{
			if (this.workAmount >= 0f)
			{
				return this.workAmount;
			}
			return this.products[0].thingDef.GetStatValueAbstract(StatDefOf.WorkToMake, stuffDef);
		}

		[DebuggerHidden]
		public IEnumerable<ThingDef> PotentiallyMissingIngredients(Pawn billDoer, Map map)
		{
			RecipeDef.<PotentiallyMissingIngredients>c__Iterator1D7 <PotentiallyMissingIngredients>c__Iterator1D = new RecipeDef.<PotentiallyMissingIngredients>c__Iterator1D7();
			<PotentiallyMissingIngredients>c__Iterator1D.map = map;
			<PotentiallyMissingIngredients>c__Iterator1D.billDoer = billDoer;
			<PotentiallyMissingIngredients>c__Iterator1D.<$>map = map;
			<PotentiallyMissingIngredients>c__Iterator1D.<$>billDoer = billDoer;
			<PotentiallyMissingIngredients>c__Iterator1D.<>f__this = this;
			RecipeDef.<PotentiallyMissingIngredients>c__Iterator1D7 expr_2A = <PotentiallyMissingIngredients>c__Iterator1D;
			expr_2A.$PC = -2;
			return expr_2A;
		}

		public bool IsIngredient(ThingDef th)
		{
			for (int i = 0; i < this.ingredients.Count; i++)
			{
				if (this.ingredients[i].filter.Allows(th) && (this.ingredients[i].IsFixedIngredient || this.fixedIngredientFilter.Allows(th)))
				{
					return true;
				}
			}
			return false;
		}

		[DebuggerHidden]
		public override IEnumerable<string> ConfigErrors()
		{
			RecipeDef.<ConfigErrors>c__Iterator1D8 <ConfigErrors>c__Iterator1D = new RecipeDef.<ConfigErrors>c__Iterator1D8();
			<ConfigErrors>c__Iterator1D.<>f__this = this;
			RecipeDef.<ConfigErrors>c__Iterator1D8 expr_0E = <ConfigErrors>c__Iterator1D;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		public override void ResolveReferences()
		{
			base.ResolveReferences();
			for (int i = 0; i < this.ingredients.Count; i++)
			{
				this.ingredients[i].ResolveReferences();
			}
			if (this.fixedIngredientFilter != null)
			{
				this.fixedIngredientFilter.ResolveReferences();
			}
			if (this.defaultIngredientFilter == null)
			{
				this.defaultIngredientFilter = new ThingFilter();
				if (this.fixedIngredientFilter != null)
				{
					this.defaultIngredientFilter.CopyAllowancesFrom(this.fixedIngredientFilter);
				}
			}
			this.defaultIngredientFilter.ResolveReferences();
		}

		public bool PawnSatisfiesSkillRequirements(Pawn pawn)
		{
			return this.skillRequirements == null || !this.skillRequirements.Any((SkillRequirement req) => !req.PawnSatisfies(pawn));
		}

		public List<ThingDef> GetPremultipliedSmallIngredients()
		{
			if (this.premultipliedSmallIngredients != null)
			{
				return this.premultipliedSmallIngredients;
			}
			this.premultipliedSmallIngredients = (from td in this.ingredients.SelectMany((IngredientCount ingredient) => ingredient.filter.AllowedThingDefs)
			where td.smallVolume
			select td).Distinct<ThingDef>().ToList<ThingDef>();
			bool flag = true;
			while (flag)
			{
				flag = false;
				for (int i = 0; i < this.ingredients.Count; i++)
				{
					bool flag2 = this.ingredients[i].filter.AllowedThingDefs.Any((ThingDef td) => !this.premultipliedSmallIngredients.Contains(td));
					if (flag2)
					{
						foreach (ThingDef current in this.ingredients[i].filter.AllowedThingDefs)
						{
							flag |= this.premultipliedSmallIngredients.Remove(current);
						}
					}
				}
			}
			return this.premultipliedSmallIngredients;
		}
	}
}
