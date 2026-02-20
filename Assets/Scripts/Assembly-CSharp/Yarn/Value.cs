using System;
using System.Globalization;

namespace Yarn
{
	public class Value : IComparable, IComparable<Value>
	{
		public enum Type
		{
			Number = 0,
			String = 1,
			Bool = 2,
			Variable = 3,
			Null = 4
		}

		public static readonly Value NULL = new Value();

		public Type type { get; internal set; }

		internal float numberValue { get; private set; }

		internal string variableName { get; set; }

		internal string stringValue { get; private set; }

		internal bool boolValue { get; private set; }

		private object backingValue => type switch
		{
			Type.Null => null, 
			Type.String => stringValue, 
			Type.Number => numberValue, 
			Type.Bool => boolValue, 
			_ => throw new InvalidOperationException($"Can't get good backing type for {type}"), 
		};

		public float AsNumber
		{
			get
			{
				switch (type)
				{
				case Type.Number:
					return numberValue;
				case Type.String:
					try
					{
						return float.Parse(stringValue, CultureInfo.InvariantCulture);
					}
					catch (FormatException)
					{
						return 0f;
					}
				case Type.Bool:
					if (!boolValue)
					{
						return 0f;
					}
					return 1f;
				case Type.Null:
					return 0f;
				default:
					throw new InvalidOperationException("Cannot cast to number from " + type);
				}
			}
		}

		public bool AsBool
		{
			get
			{
				switch (type)
				{
				case Type.Number:
					if (!float.IsNaN(numberValue))
					{
						return numberValue != 0f;
					}
					return false;
				case Type.String:
					return !string.IsNullOrEmpty(stringValue);
				case Type.Bool:
					return boolValue;
				case Type.Null:
					return false;
				default:
					throw new InvalidOperationException("Cannot cast to bool from " + type);
				}
			}
		}

		public string AsString
		{
			get
			{
				switch (type)
				{
				case Type.Number:
					if (float.IsNaN(numberValue))
					{
						return "NaN";
					}
					return numberValue.ToString();
				case Type.String:
					return stringValue;
				case Type.Bool:
					return boolValue.ToString();
				case Type.Null:
					return "null";
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		public Value()
			: this(null)
		{
		}

		public Value(object value)
		{
			if (typeof(Value).IsInstanceOfType(value))
			{
				Value value2 = value as Value;
				type = value2.type;
				switch (type)
				{
				case Type.Number:
					numberValue = value2.numberValue;
					break;
				case Type.String:
					stringValue = value2.stringValue;
					break;
				case Type.Bool:
					boolValue = value2.boolValue;
					break;
				case Type.Variable:
					variableName = value2.variableName;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				case Type.Null:
					break;
				}
			}
			else if (value == null)
			{
				type = Type.Null;
			}
			else if (value.GetType() == typeof(string))
			{
				type = Type.String;
				stringValue = Convert.ToString(value);
			}
			else if (value.GetType() == typeof(int) || value.GetType() == typeof(float) || value.GetType() == typeof(double))
			{
				type = Type.Number;
				numberValue = Convert.ToSingle(value);
			}
			else
			{
				if (!(value.GetType() == typeof(bool)))
				{
					throw new YarnException($"Attempted to create a Value using a {value.GetType().Name}; currently, Values can only be numbers, strings, bools or null.");
				}
				type = Type.Bool;
				boolValue = Convert.ToBoolean(value);
			}
		}

		public virtual int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is Value other))
			{
				throw new ArgumentException("Object is not a Value");
			}
			return CompareTo(other);
		}

		public virtual int CompareTo(Value other)
		{
			if (other == null)
			{
				return 1;
			}
			if (other.type == type)
			{
				switch (type)
				{
				case Type.Null:
					return 0;
				case Type.String:
					return stringValue.CompareTo(other.stringValue);
				case Type.Number:
					return numberValue.CompareTo(other.numberValue);
				case Type.Bool:
					return boolValue.CompareTo(other.boolValue);
				}
			}
			return AsNumber.CompareTo(other.AsNumber);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Value value = (Value)obj;
			switch (type)
			{
			case Type.Number:
				return AsNumber == value.AsNumber;
			case Type.String:
				return AsString == value.AsString;
			case Type.Bool:
				return AsBool == value.AsBool;
			case Type.Null:
				if (value.type != Type.Null && value.AsNumber != 0f)
				{
					return !value.AsBool;
				}
				return true;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public override int GetHashCode()
		{
			return backingValue?.GetHashCode() ?? 0;
		}

		public override string ToString()
		{
			return $"[Value: type={type}, AsNumber={AsNumber}, AsBool={AsBool}, AsString={AsString}]";
		}

		public static Value operator +(Value a, Value b)
		{
			if (a.type == Type.String || b.type == Type.String)
			{
				return new Value(a.AsString + b.AsString);
			}
			if (a.type == Type.Number || b.type == Type.Number || (a.type == Type.Bool && b.type == Type.Bool) || (a.type == Type.Null && b.type == Type.Null))
			{
				return new Value(a.AsNumber + b.AsNumber);
			}
			throw new ArgumentException($"Cannot add types {a.type} and {b.type}.");
		}

		public static Value operator -(Value a, Value b)
		{
			if ((a.type == Type.Number && (b.type == Type.Number || b.type == Type.Null)) || (b.type == Type.Number && (a.type == Type.Number || a.type == Type.Null)))
			{
				return new Value(a.AsNumber - b.AsNumber);
			}
			throw new ArgumentException($"Cannot subtract types {a.type} and {b.type}.");
		}

		public static Value operator *(Value a, Value b)
		{
			if ((a.type == Type.Number && (b.type == Type.Number || b.type == Type.Null)) || (b.type == Type.Number && (a.type == Type.Number || a.type == Type.Null)))
			{
				return new Value(a.AsNumber * b.AsNumber);
			}
			throw new ArgumentException($"Cannot multiply types {a.type} and {b.type}.");
		}

		public static Value operator /(Value a, Value b)
		{
			if ((a.type == Type.Number && (b.type == Type.Number || b.type == Type.Null)) || (b.type == Type.Number && (a.type == Type.Number || a.type == Type.Null)))
			{
				return new Value(a.AsNumber / b.AsNumber);
			}
			throw new ArgumentException($"Cannot divide types {a.type} and {b.type}.");
		}

		public static Value operator %(Value a, Value b)
		{
			if ((a.type == Type.Number && (b.type == Type.Number || b.type == Type.Null)) || (b.type == Type.Number && (a.type == Type.Number || a.type == Type.Null)))
			{
				return new Value(a.AsNumber % b.AsNumber);
			}
			throw new ArgumentException($"Cannot modulo types {a.type} and {b.type}.");
		}

		public static Value operator -(Value a)
		{
			if (a.type == Type.Number)
			{
				return new Value(0f - a.AsNumber);
			}
			if (a.type == Type.Null && a.type == Type.String && (a.AsString == null || a.AsString.Trim() == ""))
			{
				return new Value(0);
			}
			return new Value(float.NaN);
		}

		public static bool operator >(Value operand1, Value operand2)
		{
			return operand1.CompareTo(operand2) == 1;
		}

		public static bool operator <(Value operand1, Value operand2)
		{
			return operand1.CompareTo(operand2) == -1;
		}

		public static bool operator >=(Value operand1, Value operand2)
		{
			return operand1.CompareTo(operand2) >= 0;
		}

		public static bool operator <=(Value operand1, Value operand2)
		{
			return operand1.CompareTo(operand2) <= 0;
		}
	}
}
