public interface IStmtVisitor
{
    void VisitSpawn(SpawnCommand cmd);
    void VisitColor(ColorCommand cmd);
    void VisitSize(SizeCommand cmd);
    void VisitDrawLine(DrawLineCommand cmd);
    void VisitDrawCircle(DrawCircleCommand cmd);
    void VisitDrawRectangle(DrawRectangleCommand cmd);
    void VisitFill(FillCommand cmd);

    void VisitLabel(LabelExpression lbl);
    void VisitGoto(GotoCommand gt);

    void VisitAssign(AssignExpression assign);
}
