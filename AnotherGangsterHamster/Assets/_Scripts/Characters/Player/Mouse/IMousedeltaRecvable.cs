namespace Characters.Player.Mouse
{
   public interface IMousedeltaRecvable
   {
      /// <summary>
      /// 마우스 X 의 delta 를 전달
      /// </summary>
      public void OnMouseX(float x);

      /// <summary>
      /// 마우스 Y 의 delta 를 전달
      /// </summary>
      public void OnMouseY(float y, bool includingMouseSpeed = true);
   }
}