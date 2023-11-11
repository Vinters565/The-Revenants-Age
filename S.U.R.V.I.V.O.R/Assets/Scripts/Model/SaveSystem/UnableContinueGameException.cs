using System;
namespace TheRevenantsAge
{
    public class UnableContinueGameException : Exception
    {
        public override string Message => "Игра не может быть продолжена!";
    }
}