namespace Todo.Attribute
{
    /// <summary>
    /// Todo Attribute class to mark methods or classes that need further implementation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method
            | AttributeTargets.Class
            | AttributeTargets.Property
            | AttributeTargets.Constructor)]
    public class TodoAttribute : System.Attribute
    {
        /// <summary>
        /// message describing the todo item. [Required]
        /// </summary>
        public string Message { get; }

        #region Constructor
        /// <summary>
        /// Constructor for TodoAttribute.
        /// </summary>
        public TodoAttribute(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException("A message is required ", nameof(message));
            }

            Message = message;
        }
        #endregion

        /// <summary>
        /// Optional owner of the todo item.
        /// </summary>
        public string? owner { get; set; }

        public int? priority { get; set; }

        // You can add more properties as needed
        // Just follow the same pattern and ensure they are optional
    }
}
