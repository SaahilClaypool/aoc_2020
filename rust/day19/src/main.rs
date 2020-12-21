use std::{collections::HashMap, fs};

fn main() {
    println!("Hello, world!");

    let input = "./small_input.txt";
    let input = "./sample.txt";
    let input = "./input.txt";
    let contents = fs::read_to_string(input).expect("Something went wrong reading the file");

    let (rules, lines) = parse(&contents);

    println!("{}", contents);
    println!("{:?}", rules);

    println!("matching..");
    let matching_lines: Vec<&str> = lines
        .into_iter()
        .filter(|line| {
            let mut visited = &mut HashMap::new();
            let m: Vec<usize> = matches(0, line, 0, &rules, &mut visited)
                .into_iter()
                .filter(|m| *m == line.len())
                .collect();
            m.len() > 0
        })
        .collect();

    // dbg!(visited);
    println!("number of matches: {}", matching_lines.len());
}

#[derive(Debug)]
enum Rule {
    Reference(Vec<Vec<usize>>),
    Rule(char),
}

fn state_string(input: &str, idx: usize, rule: usize) -> String {
    format!("{}_{}", rule, &input[idx..])
}

/// return list of possible indexes after running stirng through rules
fn matches<'a>(
    ridx: usize,
    input: &'a str,
    input_idx: usize,
    rules: &HashMap<usize, Rule>,
    visited: &mut HashMap<String, Vec<usize>>, // map from rule_state -> all the indexes you could end at
) -> Vec<usize> {


    if input_idx >= input.len() {
        // println!("sc: Out of bounds");
        return vec![];
    }

    let state = state_string(input, input_idx, ridx);
    match visited.get(state.as_str()) {
        Some(previous_computation) => {
            // println!("sc: previous state bubbling up {} {:?}", state.as_str(), &previous_computation);
            return previous_computation.to_vec()
        },
        None => {
            // println!("sc: temp insert into visited for rule {}", ridx);
            visited.insert(state.clone(), vec![]);
        }
    }


    let rule = rules.get(&ridx).unwrap();
    let possible_indexes = match rule {
        Rule::Rule(letter) => {
            // println!("sc: Leaf => {} @ {}", letter, input_idx);
            if *letter == input.chars().nth(input_idx).expect("out of bounds") {
                vec![input_idx + 1]
            } else {
                vec![]
            }
        }
        Rule::Reference(referenced_rules) => referenced_rules
            .iter()
            .flat_map(|rule_list| {
                // println!("sc: Arm {:?}", rule_list);
                match_list(rule_list, input, input_idx, rules, visited)
            })
            .collect(),
    };

    // println!("sc: inserting {:?} into visited for rule {}", possible_indexes.clone(), ridx);
    visited.insert(state, possible_indexes.clone());
    possible_indexes
}

fn match_list(
    rule_indexes: &[usize],
    input: &str,
    input_idx: usize,
    rules: &HashMap<usize, Rule>,
    visited: &mut HashMap<String, Vec<usize>>,
) -> Vec<usize> {
    let mut current_indexes = vec![input_idx];
    for rule_idx in rule_indexes {
        current_indexes = current_indexes
            .into_iter()
            .flat_map(|start| {
                matches(*rule_idx, input,   start, rules, visited)
            })
            .collect();
    }

    return current_indexes;
}

fn parse(input: &str) -> (HashMap<usize, Rule>, Vec<&str>) {
    let input_lines: &str = input.split("\n\n").collect::<Vec<&str>>()[1];
    let rule_lines: &str = input.split("\n\n").collect::<Vec<&str>>()[0];
    let rules: HashMap<usize, Rule> = rule_lines
        .split("\n")
        .map(|line| {
            let words: Vec<&str> = line.split(":").collect();
            let rule_idx: usize = words[0].trim().parse().unwrap();
            let rule_type = words[1].trim();
            let rule = if rule_type.starts_with("\"") {
                Rule::Rule(rule_type.replace("\"", "").chars().next().unwrap())
            } else {
                Rule::Reference(
                    rule_type
                        .split("|")
                        .map(|ruleset| {
                            ruleset
                                .trim()
                                .split(" ")
                                .map(|index| index.trim().parse().unwrap())
                                .collect()
                        })
                        .collect(),
                )
            };
            (rule_idx, rule)
        })
        .collect();
    (rules, input_lines.split("\n").collect())
}