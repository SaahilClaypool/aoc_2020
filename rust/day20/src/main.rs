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

fn dfs(mut tiles: Vec<Tile>) {}

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
        let mut pieces: HashMap<i32, Tile> =
            self.tiles.clone().into_iter().map(|t| (t.id, t)).collect();
        let mut puzzle: HashMap<Loc, Tile> = HashMap::new();

        let mut edges = pieces
            .iter()
            .filter(|(_k, t)| {
                let matches = self.possible_matches(t);
                matches.len() < 4
            })
            .collect::<Vec<_>>();

        let (_, corner) = edges.pop().unwrap();
        puzzle.insert(Loc { row: 0, col: 0 }, corner.clone());

        while puzzle.values().len() < edges.len() {
            for tile in 0..edges.len() {
                let tile = edges[tile].1;
                if puzzle.values().any(|v| v.id == tile.id) {
                    continue;
                }
                let placed = puzzle.clone();

                for placed in placed.values() {
                    if let Some((rotation, side, other_side)) = tile.fit(placed) {
                        let mut tile = (*tile).clone();
                        tile.rotation = rotation;
                        tile.loc = Self::place_location(&placed.loc, other_side);
                        println!(
                            "Fit {:?} side {:?} rotation {:?} with {:?} on its {:?} side --> {:?}",
                            tile.id, side, rotation, placed.id, other_side, tile.loc
                        );
                        if puzzle.contains_key(&tile.loc) {
                            dbg!(
                                &placed.loc,
                                other_side,
                                tile.id,
                                placed.id,
                                &puzzle[&tile.loc].id,
                                tile.rotation,
                                placed.rotation,
                                &puzzle[&tile.loc].rotation
                            );
                            panic!("Already a piece at this location");
                        } else {
                            println!(
                                "Placing {:?} {:?} into {:?} next to {}",
                                tile.id, tile.rotation, tile.loc, placed.id
                            );
                            puzzle.insert(tile.loc, tile);
                            Self::show(&puzzle);
                        }
                    }
                }
            }
        }

        dbg!(puzzle.values().count());

        let mut centers = pieces
            .iter()
            .filter(|(_k, t)| {
                let matches = self.possible_matches(t);
                matches.len() == 4
            })
            .collect::<Vec<_>>();

        dbg!(centers.len(), edges.len());
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
            Rotation::Left => Loc {
                row: loc.row,
                col: loc.col - 1,
            },
            Rotation::Right => Loc {
                row: loc.row,
                col: loc.col + 1,
            },
            Rotation::Up => Loc {
                row: loc.row - 1,
                col: loc.col,
            },
            Rotation::Down => Loc {
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
}

impl Rotation {
    /// if self wants to line up with their Rotation,
    /// what rotation should self be in
    fn match_rotation(&self, other: &Rotation) -> Self {
        let other_number: usize = *other as usize;
        let this_number: usize = *self as usize;

        ((6 + other_number - this_number) % 4).into()
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
        } else {
            panic!(format!("can't decode rotation {}", u));
        }
    }
}

#[derive(Debug, Clone)]
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

        for r in 0..4 {
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
        edges.rotate_right(rotation as usize);
        vec![
            edges.remove(0),
            edges.remove(0),
            Self::flip(edges.remove(0)),
            Self::flip(edges.remove(0)),
        ]
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
        || a.iter().rev().zip(b).all(|(a, b)| a == b) && a.len() == b.len()
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
