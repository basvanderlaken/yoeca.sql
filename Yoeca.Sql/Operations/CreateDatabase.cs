using JetBrains.Annotations;

namespace Yoeca.Sql
{
    public sealed class CreateDatabase
    {
        public readonly string Name;

        private CreateDatabase([NotNull] string name)
        {
            Name = name;
        }

        [NotNull]
        public static CreateDatabase WithName([NotNull] string name)
        {
            return new CreateDatabase(name);
        }

        public override string ToString()
        {
            return "CREATE DATABASE " + Name;
        }
    }
}