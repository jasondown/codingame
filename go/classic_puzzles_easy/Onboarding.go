// https://www.codingame.com/training/easy/onboarding

package main

import "fmt"

func main() {
	for {
		var enemy1 string
		fmt.Scan(&enemy1)

		var dist1 int
		fmt.Scan(&dist1)

		var enemy2 string
		fmt.Scan(&enemy2)

		var dist2 int
		fmt.Scan(&dist2)

		if dist1 < dist2 {
			fmt.Println(enemy1)
		} else {
			fmt.Println(enemy2)
		}
	}
}
