namespace MosaicDroid.Core
{
    public static class ArgumentRegistry
    {
        private static readonly Dictionary<string, ArgumentSpec> _specs = new()
        {
            // Instructions:
            [TokenValues.Spawn] = new ArgumentSpec(2, ExpressionType.Number, ExpressionType.Number),
            [TokenValues.Color] = new ArgumentSpec(1, ExpressionType.Text),
            [TokenValues.Size] = new ArgumentSpec(1, ExpressionType.Number),
            [TokenValues.DrawLine] = new ArgumentSpec(3, ExpressionType.Number, ExpressionType.Number, ExpressionType.Number),
            [TokenValues.DrawCircle] = new ArgumentSpec(3, ExpressionType.Number, ExpressionType.Number, ExpressionType.Number),
            [TokenValues.DrawRectangle] = new ArgumentSpec(5,
                                            ExpressionType.Number, ExpressionType.Number, ExpressionType.Number,
                                            ExpressionType.Number, ExpressionType.Number),
            [TokenValues.Fill] = new ArgumentSpec(0),
            [TokenValues.Move] = new ArgumentSpec(2, ExpressionType.Number, ExpressionType.Number),

            // Functions:
            [TokenValues.GetActualX] = new ArgumentSpec(0),
            [TokenValues.GetActualY] = new ArgumentSpec(0),
            [TokenValues.GetCanvasSize] = new ArgumentSpec(0),
            [TokenValues.GetColorCount] = new ArgumentSpec(5,
                                            ExpressionType.Text,
                                            ExpressionType.Number, ExpressionType.Number,
                                            ExpressionType.Number, ExpressionType.Number),
            [TokenValues.IsBrushColor] = new ArgumentSpec(1, ExpressionType.Text),
            [TokenValues.IsBrushSize] = new ArgumentSpec(1, ExpressionType.Number),
            [TokenValues.IsCanvasColor] = new ArgumentSpec(3, ExpressionType.Text, ExpressionType.Number, ExpressionType.Number),
        };

        public static ArgumentSpec? Get(string name)
            => _specs.TryGetValue(name, out var spec) ? spec : null;
    }
}