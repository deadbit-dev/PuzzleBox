#ifndef SHADER_UTILS
#define SHADER_UTILS

float rounded_rectangle(const float2 uv, const float width, float height, float radius, const bool antialiasing)
{
    radius = max(min(min(abs(radius * 2), abs(width)), abs(height)), 1e-5);
    const float2 corner = abs(uv * 2 - 1) - float2(width, height) + radius;
    const float d = length(max(0, corner)) / radius;
    return antialiasing ? saturate((1 - d) / fwidth(d)) : step(0, 1 - d);
}

float2 polar_coordinates(const float2 uv, const float2 center, const float radial_scale, const float length_scale)
{
    float2 delta = uv - center;
    float radius = length(delta) * 2 * radial_scale;
    float angle = atan2(delta.x, delta.y) * 1.0/6.28 * length_scale;
    return float2(radius, angle);
}

void outline_for_rounded_rectangle(in const float2 uv, in const float width, in const float height, in const float radius, in const float thickness, in const float segments, in const float dash_length, in const float gap_length, out float Out)
{
    const float out_rectangle = rounded_rectangle(uv, width, height, radius, true);
    const float in_rectangle = rounded_rectangle(uv, width - thickness, height - thickness, radius - thickness * 0.5, true);
    
    const float outline = out_rectangle - in_rectangle;
    
    const float2 polar_coordinate = polar_coordinates(uv, float2(0.5, 0.5), 0, segments);
    const float segment = step(0.5, frac(polar_coordinate.y));

    Out = (segments == 0) ? outline : outline && segment;
    
    
    // // Normalize the uv coordinates relative to the center of the rectangle
    // const float2 uv = (uv * 2 - 1) * float2(width / 2,  height / 2);
    //
    // // Determine the distance from the point to the nearest edge of the rectangle
    // const float dist = min(
    //     min(abs(uv.x - width / 2), abs(uv.x + width / 2)),
    //     min(abs(uv.y - height / 2), abs(uv.y + height / 2))
    // );
    //
    // // Determine the angle between the point and the center of the rectangle
    // const float angle = atan2(uv.y, uv.x);
    //
    // // Determine the length of the arc of a circle with radius dist and angle
    // const float arc_length = dist * angle;
    //
    // // Determine whether the point is on the arc of a rounded corner or on the straight side of the rectangle
    // const bool on_arc = (abs(uv.x) > width / 2 - radius) && (abs(uv.y) > height / 2 - radius);
    //
    // // Determine whether the point is on a dash or a gap
    // bool on_dash;
    // if (on_arc) on_dash = frac(arc_length / (dash_length + gap_length)) < dash_length / (dash_length + gap_length);
    // else on_dash = frac(abs(uv.x) + abs(uv.y)) / (dash_length + gap_length) < dash_length / (dash_length + gap_length);
    //
    // // Return the result depending on the distance and the dash
    // Out = dist < 0.01 && on_dash ? 1 : 0;
}


#endif