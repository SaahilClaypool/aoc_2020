use std::{fmt::DebugStruct, fs};

fn main() {
    println!("Hello, world!");

    let input = "sample.txt";
    let tiles = parse(input);
    dbg!(tiles);
}

#[derive(Debug)]
enum Rotation {
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3,
}

#[derive(Debug)]
struct Tile {
    id: String,
    pixels: Vec<Vec<char>>,
    _edges: Vec<Vec<char>>,
    rotation: Rotation,
}

impl Tile {
    fn new(id: String, pixels: Vec<Vec<char>>) -> Self {
        let mut _self = Tile {
            id,
            pixels,
            _edges: vec![],
            rotation: Rotation::Up,
        };
        _self.cache_edges();

        _self
    }

    fn edges(&self, rotation: Rotation) -> Vec<Vec<char>> {
        let mut edges = self._edges.clone();
        edges.rotate_right(rotation as usize);
        edges
    }

    fn cache_edges(&mut self) {
        self._edges = self.compute_edges();
    }

    fn compute_edges(&self) -> Vec<Vec<char>> {
        let mut edges = vec![vec![], vec![], vec![], vec![]];
        for (row_num, row) in self.pixels.iter().enumerate() {
            for (col_num, col) in row.iter().enumerate() {
                if row_num == 0 {
                    edges[0].push(*col);
                }
                if col_num == row.len() - 1 {
                    edges[1].push(*col);
                }
                if row_num == self.pixels.len() - 1 {
                    edges[2].push(*col);
                }
                if col_num == 0 {
                    edges[3].push(*col);
                }
            }
        }

        edges
    }
}

fn parse(f: &str) -> Vec<Tile> {
    let f = fs::read_to_string(f).expect("couldn't open file");
    f.split("\n\n")
        .map(|tile| {
            let mut parts = tile.split("\n");
            let id = String::from(parts.next().unwrap());
            let pixels: Vec<Vec<char>> = parts.map(|col| col.chars().collect()).collect();
            Tile::new(id, pixels)
        })
        .collect()
}
