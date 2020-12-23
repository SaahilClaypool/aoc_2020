use std::collections::{hash_map::Entry, HashMap, HashSet};

fn main() {
    println!("Hello, world!");
    let input = "sample.txt";
    let (mut foods, mut rules) = parse(input);
    let mut solved_foods = HashMap::new();
    let mut impossible_foods = HashSet::new();

    loop {
        // remove impossible foods
        let food;
        let allergen;
        if let Some(impossible_food) = find_impossible_food(&foods, &solved_foods, &rules) {
            allergen = impossible_food.1.clone();
            food = impossible_food.0.clone();
            dbg!(&food);
        } 

        foods.remove(&food);
        for (food_rules, _allergens) in rules.iter_mut() {
            *food_rules = food_rules.iter().filter(|f| **f != food).map(|f| f.clone()).collect();
        }
        impossible_foods.insert(food);

        // find viable solveable foods
        let vfood;
        let vallergen;
        if let Some(viable_food) = find_solved_food(&foods, &solved_foods, &rules) {
            vallergen = viable_food.1.clone();
            vfood = viable_food.0.clone();
            dbg!(&vfood);
        } else {
            break;
        }

    }
}


fn find_solved_food<'a>(
    foods: &'a HashMap<String, HashSet<String>>,
    solved: &'a HashMap<String, String>,
    rules: &'a Vec<(Vec<String>, Vec<String>)>,
) -> Option<(&'a String, &'a String)> {
    foods.iter()
        .find(|(_f, a)| a.len() == 1)
        .map(|(f, a)| (f, a.iter().next().unwrap()))
}

fn find_impossible_food<'a>(
    foods: &'a HashMap<String, HashSet<String>>,
    solved: &'a HashMap<String, String>,
    rules: &'a Vec<(Vec<String>, Vec<String>)>,
) -> Option<(&'a String, &'a String)> {
    // go through each rul

    let viables: Vec<_> = foods.iter().filter(|(f, a)| a.len() == 1).collect();
    dbg!(&viables);
    for (viable_food, allergens) in viables {
        let mut new_foods = foods.clone();
        let mut solved = solved.clone();
        let allergen = allergens.iter().next().unwrap();
        solved.insert(viable_food.clone(), allergen.clone());

        dbg!(viable_food, &allergens, &solved);
        for (f, allergens) in new_foods.iter_mut() {
            allergens.remove(viable_food);
        }

        let solved_allergens = solved.values().collect::<Vec<_>>();
        let solved_foods = solved.keys().collect::<Vec<_>>();
        let invalid_rules = rules.iter().filter(|(foods, allergens)| {
            allergens.iter().filter(|a| !solved_allergens.contains(a)).count() == 0 &&
            foods.iter().filter(|f| !solved_foods.contains(f)).count() != 0
        }).collect::<Vec<_>>();

        dbg!(&invalid_rules);
        if invalid_rules.len() != 0 {
            return Some((viable_food, allergen));
        }
    }
    None
}

fn non_allergen_foods<'a>(
    foods: &'a HashMap<String, HashSet<String>>,
    solved: &HashMap<String, String>,
) -> Vec<&'a String> {
    foods
        .iter()
        .filter(|(f, _a)| !solved.contains_key(*f))
        .map(|(f, _a)| f)
        .collect::<Vec<&String>>()
}

fn count_occrences(input: &str, foods: &Vec<&String>) -> HashMap<String, i32> {
    let content = std::fs::read_to_string(input).expect("no such file");
    let mut ret = HashMap::new();
    for line in content.split("\n") {
        for food in foods {
            if line.split(" ").any(|w| w == *food) {
                let entry = match ret.entry(String::from(*food)) {
                    Entry::Occupied(o) => o.into_mut(),
                    Entry::Vacant(v) => v.insert(0),
                };

                *entry += 1;
            }
        }
    }

    ret
}

fn parse(
    input_file: &str,
) -> (
    HashMap<String, HashSet<String>>,
    Vec<(Vec<String>, Vec<String>)>,
) {
    let content = std::fs::read_to_string(input_file).expect("no such file");

    let lines = content.split("\n");

    let mut input = HashMap::new();
    let mut rules = vec![];

    for line in lines {
        let mut parts = line.split("(contains ");
        dbg!(line);
        let keys = parts
            .nth(0)
            .unwrap()
            .split(" ")
            .map(|w| w.trim())
            .filter(|w| w.len() > 0)
            .collect::<Vec<_>>();
        let allergens = parts
            .nth(0)
            .unwrap()
            .split(", ")
            .map(|a| String::from(a.replace("(", "").replace(")", "").trim()));

        for key in keys.iter() {
            let entry = match input.entry(String::from(*key)) {
                Entry::Occupied(o) => o.into_mut(),
                Entry::Vacant(v) => v.insert(HashSet::new()),
            };

            entry.extend(allergens.clone());
        }
        rules.push((
            keys.iter().map(|k| String::from(*k)).collect(),
            allergens.collect(),
        ));
    }

    (input, rules)
}
