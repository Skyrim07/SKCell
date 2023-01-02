namespace SKCell
{
    /// <typeparam name="TS">State</typeparam>
    /// <param name="state">certain state</param>
    public delegate void FSMOnStateChangeEvt<TS>(TS state);
    public delegate void FSMOnTransitEvt<TS>(TS from, TS to);
    public delegate void FSMUpdateEvt();
    public delegate bool FSMCondition();
}