﻿using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace LinqToDB.Linq.Parser.Clauses
{
	public sealed class Sequence : BaseClause
	{
		public List<BaseClause> Clauses { get; set; }

		public Sequence()
		{
			Clauses = new List<BaseClause>();
		}

		private Sequence([NotNull] List<BaseClause> clauses)
		{
			Clauses = clauses ?? throw new ArgumentNullException(nameof(clauses));
		}

		public void AddClause(BaseClause clause)
		{
			Clauses.Add(clause);
		}

		public IQuerySource GetQuerySource()
		{
			for (var i = Clauses.Count - 1; i >= 0; i--)
			{
				var clause = Clauses[i];
				if (clause is IQuerySource qs)
					return qs;
			}

			return null;
		}

		public override BaseClause Visit(Func<BaseClause, BaseClause> func)
		{
			var clauses = VisitList(Clauses, func);
			if (clauses != Clauses) 
				return new Sequence(clauses);
			return func(this);
		}

		public override bool VisitParentFirst(Func<BaseClause, bool> func)
		{
			return func(this) &&
			       VisitListParentFirst(Clauses, func);
		}
	}
}