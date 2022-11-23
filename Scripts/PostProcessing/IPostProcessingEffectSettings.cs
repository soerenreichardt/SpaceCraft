namespace PostProcessing
{
    public interface IPostProcessingEffectSettings<in TFixedData, in TUpdatableData>
    {
        void SetProperties();
        void SetFixedData(TFixedData data);
        void SetUpdatableData(TUpdatableData data);
    }
}