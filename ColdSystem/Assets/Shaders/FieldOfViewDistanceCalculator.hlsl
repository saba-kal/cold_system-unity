//UNITY_SHADER_NO_UPGRADE
#ifndef MYHLSLINCLUDE_UNLCUDED
#define MYHLSLINCLUDE_UNLCUDED

uniform int _RayCount = 64;
uniform int _RayOriginCount = 5;
uniform int _RayOffset = 64;
uniform float3 _RayStartPositions[5];
uniform float3 _RayEndPositions[2000];


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
        Out = length(position - lineStart); // Rounded endpoints.
        // Out = 1000; // Hard endpoints.
    }
    else if (T >= 1.0)
    {
        // Closest point is the line end
        Out = length(position - lineEnd); // Rounded endpoints.
        // Out = 1000; // Hard endpoints.
    }
    else
    {
        // Closest point is on the line segment
        float3 projectedPoint = lineStart + T * lineVector;
        Out = length(projectedPoint - position);
    }
}

void CalculateDistanceToFieldOfView_float(float3 position, float distanceThreshold, out float Out, out float T, out float lineLength) 
{
    Out = 1000.0;
    T = 1000.0;
    lineLength = 1000.0;
    int rayStartIndex = 0;
    for (int i = 0; i < _RayOriginCount; i++) {
        float3 lineStart = _RayStartPositions[i];

        for (int j = rayStartIndex; j < rayStartIndex + _RayOffset; j++) {
            float3 lineEnd = _RayEndPositions[j];
            float dist;
            float t;
            DistanceToLine_float(position, lineStart, lineEnd, dist, t);
            if (dist < Out) 
            {
                Out = dist;
                T = t;
                lineLength = distance(lineStart, lineEnd);
            }
        }
        rayStartIndex += _RayOffset;
    }
}


#endif //MYHLSLINCLUDE_UNLCUDED