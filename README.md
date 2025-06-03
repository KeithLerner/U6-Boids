# Unity6 Boids
I challenged myself to create boids on a plane ride with no documentation and no place to plug in my laptop. The challenge went so well that I spent an additional week optimizing it before migrating the project to [c++ with Raylib](https://github.com/KeithLerner/Boids_Raylib_CPP/tree/main).

This project makes use of spatial partitions to reduce the number of neighbors any boid checks against. See the [grid bins script](https://github.com/KeithLerner/U6-Boids/blob/main/Boids/Assets/_Scripts/GridBins.cs) to learn more about how the system works. 

## Next Steps
I am currently exploring ways to implement the Burst compiler and Jobs systems to leverage Unity's DOTS.