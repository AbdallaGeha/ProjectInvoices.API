namespace ProjectInvoices.API.Domain.Enums
{
    /// <summary>
    /// Represents the current state of a construction project
    /// </summary>
    public enum ProjectState
    {
        /// <summary>
        /// The project is new or under construction 
        /// </summary>
        New = 1,

        /// <summary>
        /// The project has been executed
        /// </summary>
        Executed = 2
    }
}
