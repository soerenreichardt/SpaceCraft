namespace PostProcessing
{
    public interface IShaderSettings<in TFixedData, in TUpdatableData>
    {
        void SetProperties();
        void SetFixedData(TFixedData data);
        void SetUpdatableData(TUpdatableData data);
    }
}