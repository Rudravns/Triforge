using System.Runtime.InteropServices;

namespace csgame
{
    public static class CppIntegration
    {
        private const string DLL_PATH =
            @"C:\Users\Rudransh\Desktop\python\FPS\src\backend\out\build\x64-debug\backend\backend.dll";

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CheckRectCollision(
            float ax,
            float ay,
            float aw,
            float ah,

            float bx,
            float by,
            float bw,
            float bh
        );
    }
}