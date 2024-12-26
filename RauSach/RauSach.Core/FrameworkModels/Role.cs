using AspNetCore.Identity.Mongo.Model;

namespace RauSach.Core.FrameworkModels
{
    public class Role : MongoRole
    {
        /// <summary>
        /// Default constructor for the Role class.
        /// Initializes a new instance of the Role class with an empty name.
        /// </summary>
        /// <remarks>
        /// This constructor is used when creating a new Role object without specifying a name.
        /// It invokes the base class constructor MongoRole() with an empty string as the name parameter.
        /// </remarks>
        public Role() : base()
        {
            // The base class constructor is called with an empty string as the name parameter.
            // This initializes the Role object with an empty name, indicating that it has not been assigned a name yet.
        }

        public Role(string name) : base(name)
        {
        }
    }
}
