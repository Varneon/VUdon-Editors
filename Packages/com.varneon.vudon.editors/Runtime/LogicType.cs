namespace Varneon.VUdon.Editors
{
    public enum LogicType
    {
        /// <summary>
        /// All are true
        /// </summary>
        AND,
        /// <summary>
        /// Either is true
        /// </summary>
        OR,
        /// <summary>
        /// Either is false
        /// </summary>
        NAND,
        /// <summary>
        /// All are false
        /// </summary>
        NOR,
        /// <summary>
        /// Only one is true
        /// </summary>
        XOR,
        /// <summary>
        /// All are equal
        /// </summary>
        XNOR
    }
}
