namespace SocialApp.API.Models
{
    public class Value
    {
        //2 properties
        public int Id { get; set; }
        public string Name { get; set; }

        //Entity Framework is the object relational mapper
        //responsible for scaffolding and creating and querying our database
        //it needs to know about our models / entities

        //this class Value.cs is our model class
    }
}