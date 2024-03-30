namespace Yoeca.Sql
{
    public sealed class CreateDatabase
    {
        public readonly string Name;

        private CreateDatabase(string name)
        {
            Name = name;
        }

        public static CreateDatabase WithName(string name)
        {
            return new CreateDatabase(name);
        }

        public override string ToString()
        {
            return "CREATE DATABASE " + Name;
        }
    }
}