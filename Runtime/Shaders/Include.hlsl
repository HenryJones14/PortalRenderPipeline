struct Ray
{
    float3 origin;
    float3 direction;
};

struct Sphere
{
    float3 center;
    float radius;
};

struct Bounds
{
    float3 min;
    float3 max;
};

float RayTraceSphere(Ray ray, Sphere sphere)
{
    float3 oc = ray.origin - sphere.center;

    float a = dot(ray.direction, ray.direction);
    float b = 2.0 * dot(oc, ray.direction);
    float c = dot(oc, oc) - sphere.radius * sphere.radius;

    float discriminant = b * b - 4 * a * c;
    if (discriminant < 0)
    {
        return 0; // No intersection
    }

    float sqrtDiscriminant = sqrt(discriminant);
    float t1 = (-b - sqrtDiscriminant) / (2 * a);
    float t2 = (-b + sqrtDiscriminant) / (2 * a);

    // If t1 and t2 are both outside the ray (negative), there is no intersection
    if (t1 < 0 && t2 < 0)
    {
        return 0;
    }

    // Clamp t1 and t2 to zero if they are negative (meaning intersection starts inside the sphere)
    t1 = max(t1, 0);
    t2 = max(t2, 0);

    return t2 - t1; // Distance inside the sphere
}

float2 RayTraceBounds(Ray ray, Bounds bounds)
{
    float3 t0 = (bounds.min - ray.origin) / ray.direction;
    float3 t1 = (bounds.max - ray.origin) / ray.direction;
    
    float3 tmin = min(t0, t1);
    float3 tmax = max(t0, t1);
    
    float distA = max(max(tmin.x, tmin.y), tmin.z);
    float distB = min(tmax.x, min(tmax.y, tmax.z));

    float distance = max(0, distA);
    float length = max(0, distB - distance);

    return float2(distance, length);
}