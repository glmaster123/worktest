## Pre-challenge Summary
My plan to solve this problem is to use my knowledge of trigonometry and geometry. Specifically the parametric equation of a circle, pythagorean theorem, and various trigonometric identities.

## Identified Problems
The first problem I see is that at a certain point, turning left will result in an increasing steering angle while `x` begins to increase as well. This inflection point is approximately `0.63` inches (see scanned images) which translates to `0.72` radians. Without mechanically limiting the piston beyond a steering angle of `0.72` radians, or including other sensors to determine this state, values of `x` will yield ambiguous results.

The second problem I see is that the total piston length has a maximum value so another mechanical limit will have to be applied in order to prevent it from breaking, assuming it is not a load bearing component. The limits described thus far would make sense in practical applications anyway. This would be translated to software using if statements and throwing an error so the user knows the reading near these limits may be unreliable.

## Solving The Challenge
I was unable to solve the problem and translate my calculations into a software solution.