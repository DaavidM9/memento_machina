public static class InputManagerMock
{
    private static float horizontalInput = 0f;

    public static void SetHorizontalInput(float value)
    {
        horizontalInput = value;
    }

    public static float GetHorizontalInput()
    {
        return horizontalInput;
    }
}
