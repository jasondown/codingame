// https://www.codingame.com/training/easy/the-descent

package main

import "fmt"

func main() {
	mountains := make([]int, 8)
	for {
		for i := 0; i < 8; i++ {
			fmt.Scan(&mountains[i])
		}

		max := mountains[0]
		index := 0
		for i, h := range mountains {
			if h > max {
				max = h
				index = i
			}
		}

		fmt.Println(index)
	}
}
