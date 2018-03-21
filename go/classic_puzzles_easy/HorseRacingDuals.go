// https://www.codingame.com/training/easy/horse-racing-duals

package main

import (
	"fmt"
	"math"
	"sort"
)

func main() {
	var count int
	fmt.Scan(&count)
	var horses = make([]int, count)
	for i := 0; i < count; i++ {
		fmt.Scan(&horses[i])
	}

	sort.Ints(horses)
	minDelta, lastHorse := math.MaxInt32, math.MaxInt32
	for _, h := range horses {
		delta := int(math.Abs(float64(h - lastHorse)))
		if delta < minDelta {
			minDelta = delta
		}
		lastHorse = h
	}

	fmt.Println(minDelta)
}
