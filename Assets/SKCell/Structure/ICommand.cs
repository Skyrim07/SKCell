
namespace SKCell
{
    /// <summary>
    /// Command interface
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Execute command
        /// </summary>
        void Execute(params float[] args);

        /// <summary>
        /// Revert command
        /// </summary>
        void Revert(params float[] args);
    }
}
