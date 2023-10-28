//UNITY_SHADER_NO_UPGRADE
#ifndef MYHLSLINCLUDE_UNLCUDED
#define MYHLSLINCLUDE_UNLCUDED

void DistanceToLine_float(float3 position, float3 lineStart, float3 lineEnd, out float Out, out float T) 
{
    if (all(lineStart == lineEnd)) 
    {
        Out = 1;
        return;
    }
    
    float3 lineVector = lineEnd - lineStart;
    float3 pointToLineStart = position - lineStart;

    // Project the point onto the line
    T = dot(pointToLineStart, lineVector) / dot(lineVector, lineVector);
    if (T <= 0.0)
    {
        // Closest point is the line start
        Out = length(position - lineStart);
    }
    else if (T >= 1.0)
    {
        // Closest point is the line end
        Out = length(position - lineEnd);
    }
    else
    {
        // Closest point is on the line segment
        float3 projectedPoint = lineStart + T * lineVector;
        Out = length(projectedPoint - position);
    }
}

#endif //MYHLSLINCLUDE_UNLCUDED