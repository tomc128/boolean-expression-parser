TOKENS = {
    'AND': 'AND',
    '.': 'AND',
    'OR': 'OR',
    '+': 'OR',
    'NOT': 'NOT',
    '!': 'NOT',
    '^': 'NOT',
    '¬': 'NOT',
    '(': 'OPEN_PAREN',
    ')': 'CLOSE_PAREN',
}

OPERATORS = ['AND', 'OR', 'NOT']

DEBUG_LOGGING = False


Table = list[list[bool | None]]
Expression = tuple[list[str], list[str]]


def debug_print(*values):
    if DEBUG_LOGGING:
        print(*values)


def print_table(expr: Expression, table: Table) -> None:
    '''Prints a table in a nice format.'''

    header = ' '.join(expr[1]) + ' | Result'
    print(header)
    print('-' * len(header))

    for row in table:
        print(' '.join([ ('1' if x else '0') for x in row[:-1] ]), '|', '1' if row[-1] else '0')



def tokenise(text: str) -> Expression:
    '''Converts a boolean expression string into a list of tokens.'''

    tokens: list[str] = []
    variables: list[str] = []
    text = text.upper()

    i: int = 0
    while True:
        if i >= len(text):
            break

        char = text[i]

        if char == ' ':
            i += 1
            continue

        if ' ' in text[i:]:
            next_word_index = text.index(' ', i)
        else:
            next_word_index = len(text) - 1

        word = text[i:next_word_index]

        debug_print(f'{char} {next_word_index} {word}')

        if word in TOKENS:
            tokens.append(TOKENS[word])
            i = next_word_index
            continue

        if char in TOKENS:
            tokens.append(TOKENS[char])
            i += 1
            continue

        if char not in variables:
            variables.append(char)

        tokens.append(f'VAR_{char}')

        i += 1

    variables.sort()

    return tokens, variables



def prepare_table(expr: Expression):
    variables = { x[0]: 0 for x in expr[1] }

    num_rows = 2 ** len(variables)
    table: Table = []

    for row in range(num_rows):
        bin_str = bin(row)[2:].zfill(len(variables)) # Remove 0b prefix
        table.append([ bool(int(x)) for x in bin_str ])
        table[row].append(None)

    return table



def simulate_full(expr: Expression, table: Table):

    tokens = expr[0]
    variables = expr[1]

    for row in table:
        for i, token in enumerate(tokens):
            if token in OPERATORS:
                result: bool | None = None

                match token:
                    case 'AND':
                        operand1_token = tokens[i - 1]
                        operand2_token = tokens[i + 1]
                        operand1 = row[variables.index(operand1_token.replace('VAR_', ''))]
                        operand2 = row[variables.index(operand2_token.replace('VAR_', ''))]
                        result = operand1 and operand2
                    case 'OR':
                        operand1_token = tokens[i - 1]
                        operand2_token = tokens[i + 1]
                        operand1 = row[variables.index(operand1_token.replace('VAR_', ''))]
                        operand2 = row[variables.index(operand2_token.replace('VAR_', ''))]
                        result = operand1 or operand2
                    case 'NOT':
                        operand1_token = tokens[i + 1]
                        operand1 = row[variables.index(operand1_token.replace('VAR_', ''))]
                        result = not operand1
                
                row[-1] = result

    return table
    


def main():
    '''Main program loop.'''

    print('Basic information:')
    print(' - inputs should be letters, case insensitive (i.e. a=A)')
    print(' - allowed gates: AND (.), OR (+), NOT (!,¬,^)')
    print(' - allowed punctuation: (, )')
    print(' - enter \'q\' to quit')

    stop = False
    while not stop:

        expr1 = tokenise('A AND B . C OR A')
        debug_print(f'Tokens: {",".join(expr1[0])}')
        debug_print(f'Variables: {",".join(expr1[1])}')

        table1 = prepare_table(expr1)
        table1 = simulate_full(expr1, table1)

        print_table(expr1, table1)


        stop = True



if __name__ == '__main__':
    main()
