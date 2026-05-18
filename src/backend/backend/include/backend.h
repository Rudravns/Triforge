#pragma once

#ifdef _WIN32
#define EXPORT __declspec(dllexport)
#else
#define EXPORT
#endif

extern "C"
{
    EXPORT bool CheckRectCollision(
        float ax, float ay, float aw, float ah,
        float bx, float by, float bw, float bh
    );
}