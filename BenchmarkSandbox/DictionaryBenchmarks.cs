// Compares Dictionary lookup performance when using System.Type keys
// versus enum keys for identifying character types.

using BenchmarkDotNet.Attributes;

namespace BenchmarkSandbox;

public enum CharacterType
{
    Warrior,
    Wizard,
    Monk,
    Rogue
}

public abstract class CharacterBase { }

public sealed class Warrior : CharacterBase { }
public sealed class Wizard : CharacterBase { }
public sealed class Monk : CharacterBase { }
public sealed class Rogue : CharacterBase { }

[MemoryDiagnoser]
[DisassemblyDiagnoser]
public class DictionaryBenchmarks
{
    private static readonly Dictionary<Type, CharacterBase> TypeKeyLookup = new(4)
    {
        [typeof(Warrior)] = new Warrior(),
        [typeof(Wizard)] = new Wizard(),
        [typeof(Monk)] = new Monk(),
        [typeof(Rogue)] = new Rogue()
    };

    private static readonly Dictionary<CharacterType, CharacterBase> CharacterTypeKeyLookup = new(4)
    {
        [CharacterType.Warrior] = new Warrior(),
        [CharacterType.Wizard] = new Wizard(),
        [CharacterType.Monk] = new Monk(),
        [CharacterType.Rogue] = new Rogue()
    };

    [Benchmark]
    public CharacterBase TypeKeyLookupTest()
    {
        var warrior = TypeKeyLookup[typeof(Warrior)];
        var wizard = TypeKeyLookup[typeof(Wizard)];
        var monk = TypeKeyLookup[typeof(Monk)];
        var rogue = TypeKeyLookup[typeof(Rogue)];
        return rogue;
    }

    [Benchmark]
    public CharacterBase CharacterTypeKeyLookupTest()
    {
        var warrior = CharacterTypeKeyLookup[CharacterType.Warrior];
        var wizard = CharacterTypeKeyLookup[CharacterType.Wizard];
        var monk = CharacterTypeKeyLookup[CharacterType.Monk];
        var rogue = CharacterTypeKeyLookup[CharacterType.Rogue];
        return rogue;
    }
}