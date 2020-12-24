use std::{collections::HashMap, fmt::DebugStruct, fs};

fn main() {
    println!("Hello, world!");

    part_a();
}

fn part_a() {
    println!("Part A");
    let input = "sample.txt";
    // let input = "input.txt";
    let mut tiles = parse(input);

    let s = Solver { tiles };
    s.solve_a();
    s.solve_puzzle();
}

struct Solver {
    tiles: Vec<Tile>,
}

impl Solver {
    fn solve_a(&self) {
        let corners = self
            .tiles
            .iter()
            .filter(|tile| {
                let matches = self.possible_matches(tile);
                println!("matches: {}", &matches.len());
                matches.len() == 2
            })
            .collect::<Vec<_>>();

        dbg!(corners.len());

        let solution: i64 = corners
            .iter()
            .map(|c| c.id)
            .fold(1 as i64, |state, val| state * val as i64);
        println!("A: {}", solution);
    }

    fn solve_puzzle(&self) {
        let mut puzzle = HashMap::<Loc, Tile>::new();
        let corners = self
            .tiles
            .iter()
            .filter(|tile| {
                let matches = self.possible_matches(tile);
                println!("matches: {}", &matches.len());
                matches.len() == 2
            })
            .collect::<Vec<_>>();
        let first_corner = *corners.first().unwrap();
        puzzle.insert(Loc { row: 0, col: 0 }, first_corner.clone());

        while puzzle.len() < self.tiles.len() {
            for tile in self.tiles.iter() {
                let matches = self.possible_matches(tile);
                if puzzle.values().any(|placed| placed.id == tile.id) ||
                    !puzzle.values().any(|tile| matches.contains(&tile)) {
                    continue;
                }

                let matching_tile = puzzle.values().find(|tile| matches.contains(&tile)).unwrap();
            }
        }
    }

    fn show(puzzle: &HashMap<Loc, Tile>) {
        let min_row = puzzle.keys().map(|l| l.row).min().unwrap();
        let max_row = puzzle.keys().map(|l| l.row).max().unwrap();
        let min_col = puzzle.keys().map(|l| l.col).min().unwrap();
        let max_col = puzzle.keys().map(|l| l.col).max().unwrap();

        for r in min_row..max_row + 1 {
            for c in min_col..max_col + 1 {
                if let Some(tile) = puzzle.get(&Loc { row: r, col: c }) {
                    print!(" {} ", tile.id);
                } else {
                    print!(" ____ ");
                }
            }
            print!("\n");
        }
    }

    fn place_location(loc: &Loc, side: Rotation) -> Loc {
        match side {
            Rotation::Left | Rotation::FlipLeft => Loc {
                row: loc.row,
                col: loc.col - 1,
            },
            Rotation::Right | Rotation::FlipRight => Loc {
                row: loc.row,
                col: loc.col + 1,
            },
            Rotation::Up  | Rotation::FlipUp => Loc {
                row: loc.row - 1,
                col: loc.col,
            },
            Rotation::Down | Rotation::FlipDown => Loc {
                row: loc.row + 1,
                col: loc.col,
            },
        }
    }

    fn possible_matches(&self, tile: &Tile) -> Vec<&Tile> {
        self.tiles
            .iter()
            .filter(|other| tile.fit_with(other))
            .collect()
    }
}

#[derive(Debug, Clone, Copy, PartialEq, Hash, Eq)]
struct Loc {
    row: i32,
    col: i32,
}

#[derive(Debug, Clone, Copy, PartialEq)]
enum Rotation {
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3,
    // to flip a piece,
    // invert top & bottom and do [top, right, left, bottom]
    FlipUp = 4,
    FlipRight = 5,
    FlipDown = 6,
    FlipLeft = 7,
}

impl Rotation {
    /// if self wants to line up with their Rotation,
    /// what rotation should self be in
    fn match_rotation(&self, other: &Rotation) -> Self {
        let other_number: usize = *other as usize;
        let this_number: usize = *self as usize;

        ((6 + other_number - this_number) % 8).into()
    }
}

impl From<usize> for Rotation {
    fn from(u: usize) -> Self {
        if u == Rotation::Up as usize {
            Rotation::Up
        } else if u == Rotation::Right as usize {
            Rotation::Right
        } else if u == Rotation::Down as usize {
            Rotation::Down
        } else if u == Rotation::Left as usize {
            Rotation::Left
        } else if u == Rotation::FlipRight as usize {
            Rotation::FlipRight
        } else if u == Rotation::FlipLeft as usize {
            Rotation::FlipLeft
        } else if u == Rotation::FlipUp as usize {
            Rotation::FlipUp
        } else if u == Rotation::FlipDown as usize {
            Rotation::FlipDown
        } else {
            panic!(format!("can't decode rotation {}", u));
        }
    }
}

#[derive(Debug, Clone, PartialEq)]
struct Tile {
    loc: Loc,
    id: i32,
    pixels: Vec<Vec<char>>,
    _edges: Vec<Vec<char>>,
    rotation: Rotation,
}

impl Tile {
    fn new(id: i32, pixels: Vec<Vec<char>>) -> Self {
        let mut _self = Tile {
            id,
            pixels,
            _edges: vec![],
            rotation: Rotation::Up,
            loc: Loc { row: 0, col: 0 },
        };
        _self.cache_edges();

        _self
    }

    /// returns rotation, edge, other edge
    fn fit(&self, other: &Tile) -> Option<(Rotation, Rotation, Rotation)> {
        if other.id == self.id {
            return None;
        }

        for r in 0..8 {
            let edges = self.edges_with_rotation(r.into());
            let others = other.edges();
            if all_eq(&edges[0], &others[2]) {
                return Some((r.into(), Rotation::Up, Rotation::Down));
            }
            if all_eq(&edges[1], &others[3]) {
                return Some((r.into(), Rotation::Right, Rotation::Left));
            }
            if all_eq(&edges[3], &others[1]) {
                return Some((r.into(), Rotation::Left, Rotation::Right));
            }
            if all_eq(&edges[2], &others[0]) {
                return Some((r.into(), Rotation::Down, Rotation::Up));
            }
        }

        return None;
    }

    fn fit_with(&self, other: &Tile) -> bool {
        self.fit(other).is_some()
    }

    fn edges(&self) -> Vec<Vec<char>> {
        self.edges_with_rotation(self.rotation.clone())
    }

    fn flip(mut edge: Vec<char>) -> Vec<char> {
        edge.reverse();
        edge
    }

    fn edges_with_rotation(&self, rotation: Rotation) -> Vec<Vec<char>> {
        let mut edges = self._edges.clone();
        edges.rotate_right(rotation as usize % 4);

        // flip piece over
        if rotation as usize >= 4 {
            vec![
                Self::flip(edges[0].clone()),
                Self::flip(edges[3].clone()),
                edges[2].clone(),
                edges[1].clone(),
            ]
        } else {
            vec![
                edges.remove(0),
                edges.remove(0),
                Self::flip(edges.remove(0)),
                Self::flip(edges.remove(0)),
            ]
        }
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

        vec![
            edges.remove(0),
            edges.remove(0),
            edges.remove(0),
            edges.remove(0),
        ]
    }
}

fn all_eq<T>(a: &Vec<T>, b: &Vec<T>) -> bool
where
    T: Eq + PartialEq,
{
    a.iter().zip(b).all(|(a, b)| a == b) && a.len() == b.len()
}

fn parse(f: &str) -> Vec<Tile> {
    let f = fs::read_to_string(f).expect("couldn't open file");
    f.split("\n\n")
        .map(|tile| {
            let mut parts = tile.split("\n");
            let id = parts
                .next()
                .unwrap()
                .split(" ")
                .nth(1)
                .unwrap()
                .replace(":", "")
                .parse()
                .unwrap();
            let pixels: Vec<Vec<char>> = parts.map(|col| col.chars().collect()).collect();
            Tile::new(id, pixels)
        })
        .collect()
}

#[cfg(test)]
mod tests {
    // Note this useful idiom: importing names from outer (for mod tests) scope.
    use super::*;

    // #[test]
    fn test_all_eq() {
        let a = vec![1, 2, 3, 4, 5];
        let b = vec![1, 2, 3, 4, 5];
        let c = vec![1, 2, 4, 5];
        assert!(all_eq(&a, &b));
        assert!(!all_eq(&b, &c));
    }

    // #[test]
    fn test_rotation_bottom_to_top() {
        let top = Rotation::Up;
        let bottom = Rotation::Down;
        let left = Rotation::Left;
        let right = Rotation::Right;

        assert_eq!(bottom.match_rotation(&top), Rotation::Up);
        assert_eq!(left.match_rotation(&top), Rotation::Left);
        assert_eq!(right.match_rotation(&top), Rotation::Right);

        assert_eq!(bottom.match_rotation(&right), Rotation::Right);
        assert_eq!(left.match_rotation(&left), Rotation::Down);
        assert_eq!(right.match_rotation(&left), Rotation::Up);
    }

    #[test]
    fn test_tile_matching() {
        let input = "sample.txt";
        let tiles: HashMap<_, _> = parse(input)
            .into_iter()
            .map(|tile| (tile.id.clone(), tile))
            .collect();

        let t_1951 = tiles.get(&1951).unwrap();
        let t_2311 = tiles.get(&2311).unwrap();
    }
}
