namespace Todo.Attribute
{
    /// <summary>
    /// Todo Attribute class to mark methods, classes, properties, fields, or constructors that need further implementation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method
        | AttributeTargets.Class
        | AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Constructor)]
    public class TodoAttribute : System.Attribute
    {

        #region Required Properties
        /// <summary>
        /// Message describing the todo item. [Required]
        /// </summary>
        public string Message { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for TodoAttribute.
        /// </summary>
        /// <param name="message">A description of what needs to be done</param>
        /// <exception cref="ArgumentNullException">Thrown when message is null or whitespace</exception>
        public TodoAttribute(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message), "A message is required");
            }

            Message = message;
        }
        #endregion

        #region Optional Properties

        /// <summary>
        /// Optional owner of the todo item (e.g., developer name or team).
        /// </summary>
        public string? Owner { get; set; }

        /// <summary>
        /// Optional priority of the todo item (e.g., 1 = high, 2 = medium, 3 = low).
        /// Null indicates no priority has been assigned.
        /// </summary>
        public int? Priority { get; set; }


        // You can add more properties as needed
        // Just follow the same pattern and ensure they are optional (nullable types)
        #endregion
    }
}
