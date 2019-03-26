using Data.Dapper.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Data.Dapper.Models {
	public class SearchFilter {
		public bool AllowGrouping { get; set; } = true;
		public ConditionType ConditionType { get; set; } = ConditionType.AND;
		public EqualityType EqualityType { get; set; } = EqualityType.EQUALS;
		[Required]
		public string Name { get; set; }
		public bool SearchLeftSide { get; set; } = false;
		public bool SearchRightSide { get; set; } = true;
		public Type Type { get; set; } = typeof(string);
		public object Value { get; set; }

		public SearchFilter() {
		}

		public SearchFilter(string name, object value, bool allowGrouping = true) {
			this.AllowGrouping = allowGrouping;
			setFilters(name, value);
		}

		public SearchFilter(string name, object value, ConditionType conditionType, bool allowGrouping = true) {
			this.AllowGrouping = allowGrouping;
			setFilters(name, value);
			this.ConditionType = conditionType;
		}

		public SearchFilter(string name, object value, EqualityType equalityType, bool searchLeftSide = false, bool searchRightSide = true, bool allowGrouping = true) {
			this.AllowGrouping = allowGrouping;
			setFilters(name, value);
			this.EqualityType = equalityType;
			this.SearchLeftSide = searchLeftSide;
			this.SearchRightSide = searchRightSide;
		}

		public SearchFilter(string name, object value, ConditionType conditionType, EqualityType equalityType, bool searchLeftSide = false, bool searchRightSide = true, bool allowGrouping = true) {
			this.AllowGrouping = allowGrouping;
			setFilters(name, value);
			this.ConditionType = conditionType;
			this.EqualityType = equalityType;
			this.SearchLeftSide = searchLeftSide;
			this.SearchRightSide = searchRightSide;
		}

		public SearchFilter(SearchFilterDTO searchFilterDTO, bool allowGrouping = true) {
			this.AllowGrouping = allowGrouping;
			this.Name = searchFilterDTO.Name;
			this.Value = searchFilterDTO.Value;//not sure this will work
			this.SearchLeftSide = searchFilterDTO.SearchLeftSide;
			this.SearchRightSide = searchFilterDTO.SearchRightSide;
			this.ConditionType = this.parseEnum<ConditionType>(searchFilterDTO.ConditionType.ToUpper(), ConditionType.AND);
			this.EqualityType = this.parseEnum<EqualityType>(searchFilterDTO.EqualityType.ToUpper(), EqualityType.LIKE);
			var type = this.parseEnum<DotNetType>(searchFilterDTO.Type.ToUpper(), DotNetType.STRING);
			this.Type = this.getTypeFromString(type.ToString().ToLower());
		}

		/// <summary>
		/// This is designed to work with a group of filters for the purpose of creating IN statements.
		/// E.G. select * from Company where Name in ('Acme', 'Contoso', 'AdventureWorks')
		/// Design Note:
		/// not sure that I should default this like this, but I can't think of another scenario when you'd do this
		/// should we do exists instead? I'm not sure, this just occurred to be that it might be more performant.
		/// Like this: select * from Company where exists (select * from Company where Name = 'Acme' or Name = 'Contoso' or Name ='AdventureWorks')
		/// Instead of this: select * from Company where Name in ('Acme', 'Contoso', 'AdventureWorks')
		/// Not sure that would even be better
		/// </summary>
		/// <param name="filters"></param>
		public SearchFilter(IEnumerable<SearchFilter> filters) {
			//Notice the use of First() here
			//I hope that the NAME, ConditionType and Type values are always the same. If not, this is probably not a valid filter.
			//Not sure what to do if it is not. I'll make the assumption that they are the same for now.
			this.Name = filters.Select(a => a.Name).First();
			//allowing the ConditionType to be set allows the ORs to come through here, which is bad. let's default to 'AND' and stay there.
			//this.ConditionType = filters.Select(a => a.ConditionType).First();
			this.Type = typeof(Array);
			var type = filters.Select(a => a.Type).First();
			if (type == typeof(DateTime)) {
				this.Type = typeof(DateTime);
			}
			this.EqualityType = filters.Select(a => a.EqualityType).First();

			var acceptableEqualityTypes = new List<EqualityType> { EqualityType.BETWEEN, EqualityType.IN, EqualityType.LIKE, EqualityType.NOT_LIKE };
			if (!acceptableEqualityTypes.Contains(this.EqualityType)) {
				var msg = @"SearchFilter is attempting to group filters because there are more than one with the same name.
							This can be done, but the only acceptable Equality Types are currently the ones listed above";
				throw new Exception(msg);
			}
			if (this.EqualityType == EqualityType.LIKE || this.EqualityType == EqualityType.NOT_LIKE) {
				if (type == typeof(string)) {
					var likeOrNotLike = this.EqualityType == EqualityType.LIKE ? "like" : "not like";
					this.Value = "(" +
						string.Join(" or ",
							filters.Select(a =>
								$" { a.Name.ToString() } { likeOrNotLike } '{ prepareForLikeQuery(a.Value.ToString(), a.SearchLeftSide, a.SearchRightSide) }'"
							)
						)
					+ ")";
					return;
				} else {
					//todo: does like even work with anything other than string?
					throw new NotImplementedException();
				}
			}
			if (this.EqualityType == EqualityType.IN) {
				this.Value = filters.Select(a => a.Value).ToArray();
				return;
			}
			this.Value = filters.Select(a => a.Value.ToString()).ToArray();
		}

		/// <summary>
		/// This will only work for types in the System namespace.
		/// Int will not work. Number types should be Int32, Int64, etc
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private Type getTypeFromString(string type) {
			if (string.IsNullOrEmpty(type)) {
				return typeof(string);
			}
			//gotta uppercase that first letter of the type.
			var typeProper = type.First().ToString().ToUpper() + String.Join("", type.Skip(1));
			//todo: this is stupid. we're better than this. - DR 2017.01.18
			if (typeProper == "Datetime") {
				typeProper = "DateTime";
			}
			return Type.GetType("System." + typeProper);
		}

		private T parseEnum<T>(string input, T defaultValue) where T : struct, IConvertible {
			if (!typeof(T).IsEnum) {
				throw new ArgumentException("T must be an enumerated type");
			}
			var result = defaultValue;
			if (!Enum.TryParse<T>(input, out result)) {
				return defaultValue;
			}
			return result;
		}

		//this was copied from SQLHelper...meh
		private string prepareForLikeQuery(string searchText, bool searchLeftSide, bool searchRightSide) {
			if (string.IsNullOrEmpty(searchText)) {
				return searchText;
			}

			var value = searchText.Replace("%", "[%]").Replace("[", "[[]").Replace("]", "[]]");

			if (searchLeftSide) {
				value = "%" + value;
			}
			if (searchRightSide) {
				value += "%";
			}

			return value;
		}

		private void setFilters(string name, object value) {
			Name = name;
			if (value == null) {
				Value = string.Empty;
				Type = typeof(string);
			} else {
				Value = value;
				Type = value.GetType();
			}
		}
	}
}