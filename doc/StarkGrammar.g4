// ------------------------------------------------------------------
// Work in progress to define a new iteration on the grammar of stark
//
// NOTE: 
// The grammar is not fully functionnal for a lexer/parser as it is sensitive to NEWLINE 
// but doing that properly in ANTLR requires quite some custom hook in the grammar
// to handle them.
// ------------------------------------------------------------------

grammar StarkGrammar;

file_declaration:
    module_declaration? 
    import_statement*
    top_level_declaration*
    ;

top_level_declaration
    : struct_declaration
    | union_declaration
    | interface_declaration
    | extension_declaration
    | global_func_declaration
    | const_declaration
    | static_declaration
    ;

module_declaration:
    visibility_modifier? MODULE module_path? identifier EOS
    ;

import_statement
    : IMPORT module_path? import_typename RIGHT_BRACE EOS
    ;

import_typename
    : identifier
    | STAR
    | LEFT_BRACE identifier (COMMA identifier)* RIGHT_BRACE
    ;

const_declaration
    : visibility_modifier? CONST identifier (COLON type_reference) EQUAL expression EOS
    ;

static_declaration
    : STATIC identifier (: type_reference) EQUAL expression EOS
    ;

generic_definition_name:
    identifier generic_parameters?
    ;

generic_name:
    identifier generic_arguments?
    ;

generic_parameters:
    GRAVE LESS_THAN generic_parameter_list GREATER_THAN
    ;

generic_arguments:
    GRAVE LESS_THAN generic_argument_list GREATER_THAN
    ;

generic_parameter_list:
    generic_parameter generic_parameter_cont*
    ;

generic_argument_list:
    generic_argument generic_argument_cont*
    ;

generic_parameter_cont:
    COMMA generic_parameter
    ;

generic_argument_cont:
    COMMA generic_argument
    ;    

generic_parameter:
    generic_parameter_name
    | lifetime
    ;

generic_argument:
    generic_argument_type
    | generic_argument_literal
    | lifetime
    ;

generic_argument_literal:
    literal
    ;

generic_argument_type:
    type_reference
    ;

generic_parameter_initializer:
    EQUAL generic_argument
    ;

generic_parameter_name: 
    identifier
    ;

lifetime:
    HASH identifier
    ;

// Can be a type name or a func name
fully_qualified_path:
    module_path? generic_name 
    ;

full_typename:
    fully_qualified_path
    ;

identifier:
    IDENTIFIER;

module_path:
    (identifier COLON_COLON)+
    ;

declaration: 
    struct_declaration
    ;

struct_declaration:
    visibility_modifier? PARTIAL? REF? MUTABLE? STRUCT generic_definition_name parameters? struct_constraint* (EOS | LEFT_BRACE struct_members RIGHT_BRACE)
    ;

interface_declaration:
    visibility_modifier? PARTIAL? INTERFACE generic_definition_name parameters? interface_constraint* (EOS | LEFT_BRACE interface_members RIGHT_BRACE)
    ;

extension_declaration:
    visibility_modifier? PARTIAL? EXTENSION generic_parameters? FOR full_typename extension_constraint*  (EOS | LEFT_BRACE extension_members RIGHT_BRACE)
    ;

union_declaration:
    visibility_modifier? UNION generic_definition_name union_constraint* LEFT_BRACE union_members RIGHT_BRACE
    ;

enum_declaration:
    visibility_modifier? ENUM identifier (COLON primitive_type)? LEFT_BRACE enum_members RIGHT_BRACE
    ;

union_constraint
    : where_constraint
    ;

where_constraint
    : WHERE (identifier | lifetime) COLON where_constraint_part (COMMA where_constraint_part)*
    ;

where_constraint_part
    : IS type_reference
    | CAN NEW 
    | HAS LIFETIME (LESS_THAN | LESS_THAN_OR_EQUAL) lifetime
    | HAS CONSTRUCTOR identifier? parameters
    ;

extension_constraint
    : implement_contraint
    | where_constraint
    ;

interface_constraint
    : extends_constraint
    | where_constraint
    ;

struct_constraint
    : implement_contraint
    | where_constraint
    ;

implement_contraint:
    IMPLEMENTS full_typename
    ;

extends_constraint:
    EXTENDS full_typename
    ;

struct_members
    : const_declaration*
      field_declaration*
      constructor_declaration_with_visibility*
      func_member_declaration_with_visibility*
    ;    

union_members
    : union_member (COMMA union_member)* COMMA?
    ; 

union_member
    : identifier parameters
    | type_reference
    ;

enum_members
    : enum_member (COMMA enum_member)* COMMA?
    ;

enum_member
    : identifier (EQUAL expression)
    ;

interface_members
    : constructor_definition*
      func_member_declaration_with_visibility*
    ;

extension_members
    : const_declaration*
      constructor_declaration_with_visibility*
      func_member_declaration_with_visibility*
    ;    

field_declaration:
    visibility_modifier? (LET | VAR) identifier COLON type_reference EOS
    ;

func_member_declaration_with_visibility:
    visibility_modifier? func_member_declaration
    ;    

global_func_declaration
    : visibility_modifier? global_func_simple_declaration
    | visibility_modifier? global_func_property_declaration
    ;        

func_member_declaration:
    func_simple_declaration
    | func_property_declaration
    | func_array_declaration
    ;

func_simple_declaration: 
    func_modifier raw_func_simple_declaration
    ;

func_property_declaration: 
    func_modifier raw_func_property_declaration
    ;

global_func_simple_declaration: 
    global_func_modifier raw_func_simple_declaration
    ;

global_func_property_declaration: 
    global_func_modifier raw_func_property_declaration
    ;    

raw_func_simple_declaration: 
    FUNC generic_definition_name parameters func_return_type? throws_constraint? func_constraint* func_body
    ;

raw_func_property_declaration: 
    FUNC generic_definition_name property_type property_constraint* property_body
    ;

func_array_declaration: 
    func_this_modifier FUNC LEFT_SQUARE_BRACKET identifier COLON type_reference RIGHT_SQUARE_BRACKET property_constraint* property_body
    ;

func_global_declaration: 
    func_global_modifier FUNC generic_definition_name parameters func_return_type? throws_constraint? func_constraint* func_body
    func_global_modifier FUNC generic_definition_name property_type property_body
    ;

func_modifier:
    PARTIAL? UNSAFE? (THIS | MUTABLE THIS)?
    ;

global_func_modifier:
    PARTIAL? UNSAFE?
    ;

func_this_modifier:
    PARTIAL? UNSAFE? (THIS | MUTABLE THIS)
    ;

func_constraint
    : where_constraint
    | requires_constraint
    ;

throws_constraint
    : THROWS full_typename (COMMA full_typename)*
    ;

property_constraint
    : requires_constraint
    ;

requires_constraint
    : REQUIRES expression
    ;

func_global_modifier:
    PARTIAL? UNSAFE?
    ;

constructor_declaration_with_visibility:
    constructor_definition_with_visibility func_body;

constructor_definition_with_visibility:
    visibility_modifier? constructor_definition;

constructor_definition:
    constructor_modifier CONSTRUCTOR identifier? parameters constructor_constraint*;

constructor_constraint
    : requires_constraint
    ;

constructor_modifier:
    PARTIAL? UNSAFE?
    ;    

func_return_type:
    MINUS_GREATER_THAN type_reference
    ;

func_body:
    func_simple_body EOS
    | property_block_body
    ;

property_body
    : func_simple_body
    | property_getter
      property_setter?
    ;

property_block_body
    : LEFT_SQUARE_BRACKET property_getter property_setter? RIGHT_SQUARE_BRACKET
    ;

property_type:
    COLON type_reference
    ;

property_getter:
    GET func_body  // GET contextual only in property getter/setter
    ;
property_setter:
    SET func_body  // SET contextual only in property getter/setter
    ;

func_simple_body:
    EQUAL_GREATER_THAN expression
    ;

func_block_body:
    block_statement
    ;

block_statement:
    LEFT_BRACE statement* RIGHT_BRACE;

statement:
    let_var_statement
    | for_statement
    | if_statement
    | while_statement
    | assign_statement
    | unsafe_statement
    | block_statement
    | expression_statement
    ;

let_var_statement:
    (LET | VAR) identifier COLON type_reference (EQUAL expression) EOS
    ;

for_statement:
    FOR identifier IN expression block_statement
    ;

if_statement:
    IF expression THEN block_statement (ELSE IF expression THEN block_statement)* (ELSE block_statement)?
    ;

while_statement:
    WHILE expression block_statement
    ;

unsafe_statement:
    UNSAFE block_statement
    ;

expression
    : expression_simple
    | IF expression_simple THEN expression_simple ELSE expression_simple
    | UNSAFE expression_simple
    ;

// TODO: order is still not fully correct for operator precedence
expression_simple
    : LEFT_PAREN expression_list RIGHT_PAREN
    | THIS
    | literal
    // left_expression
    | expression_simple expression_path
    | module_path? (identifier | method_call)
    | module_path? generic_name DOT (identifier | method_call)
    // end of left_expression
    | DOT (identifier | method_call)
    | new_expression
    | prefix=(PLUS|MINUS) expression_simple
    | prefix=(TILDE|BANG) expression_simple
    | expression_simple bop=AS expression_simple
    | expression_simple bop=(STAR|DIVIDE|MODULO) expression_simple
    | expression_simple bop=(PLUS|MINUS) expression_simple
    | expression_simple (LESS_THAN LESS_THAN | GREATER_THAN GREATER_THAN) expression_simple
    | expression_simple bop=(LESS_THAN_OR_EQUAL | GREATER_THAN_OR_EQUAL | LESS_THAN | GREATER_THAN) expression_simple
    | expression_simple bop=IS type_reference
    | expression_simple bop=(EQUAL_EQUAL | BANGL_EQUAL) expression_simple
    | expression_simple bop=BITWISE_AND expression_simple
    | expression_simple bop=BITWISE_XOR expression_simple
    | expression_simple bop=BITWISE_OR expression_simple
    | expression_simple bop=AND expression_simple
    | expression_simple bop=OR expression_simple
    | REF expression_simple
    | AND expression_simple
    | TRY expression_simple (ELSE CATCH)?
    | IGNORE expression_simple
    ;

left_expression
    : module_path? (identifier | method_call)
    | literal expression_path
    | THIS expression_path
    | LEFT_PAREN expression_list RIGHT_PAREN expression_path
    | left_expression expression_path
    ;

expression_path
    : DOT identifier
    | DOT method_call
    | LEFT_SQUARE_BRACKET expression RIGHT_SQUARE_BRACKET
    ;

expression_statement
    : expression_simple
    ;

assign_statement:
    left_expression (EQUAL | PLUS_EQUAL | MINUS_EQUAL | STAR_EQUAL | DIVIDE_EQUAL | BITWISE_AND_EQUAL | BITWISE_OR_EQUAL | BITWISE_XOR_EQUAL | GREATER_THAN GREATER_THAN_OR_EQUAL | LESS_THAN LESS_THAN_OR_EQUAL | MODULO_EQUAL) expression
    ;

new_expression:
    NEW lifetime? type_constructor
    ;

type_constructor:
    module_path? generic_name arguments
    // TODO: Add array constructor
    ;    

method_call:
    generic_name arguments
    ;

expression_list:
    expression (COMMA expression)*
    ;

arguments:
    LEFT_PAREN argument_list? RIGHT_PAREN
    ;

argument_list:
    expression argument_cont*
    ;

argument_cont:
    COMMA expression;

parameters:
    LEFT_PAREN parameter_list? RIGHT_PAREN;

parameter_list:
    parameter parameter_cont*;

parameter_cont:
     COMMA parameter;

parameter:
    parameter_name COLON type_reference;

parameter_name:
    IDENTIFIER
    ;

type_reference:
    primitive_type
    | full_typename
    | ref_type
    | mutable_type
    | array_type
    | fixed_array_type
    | slice_type
    | pointer_type
    | const_type
    ;

primitive_type:
    TYPE_BOOL | TYPE_U8 | TYPE_U16 | TYPE_U32 | TYPE_U64 | TYPE_U128 | TYPE_I8 | TYPE_I16 | TYPE_I32 | TYPE_I64 | TYPE_I128 | TYPE_F32 | TYPE_F64;

const_type:
    CONST type_reference
    ;

ref_type:
    UNIQUE? REF lifetime? type_reference
    ;

mutable_type:
    MUTABLE type_reference
    ;

array_type:
    LEFT_SQUARE_BRACKET type_reference RIGHT_SQUARE_BRACKET 
    ;

fixed_array_type:
    LEFT_SQUARE_BRACKET type_reference COMMA expression RIGHT_SQUARE_BRACKET 
    ;    

slice_type:
    TILDE type_reference
    ;

pointer_type:
    STAR type_reference
    ;

literal_integer:
    DECIMAL_LITERAL | HEX_LITERAL | OCT_LITERAL | BINARY_LITERAL;

literal_float:
    FLOAT_LITERAL;

literal_bool:
    BOOL_LITERAL;

literal_char:
    CHAR_LITERAL
    ;

literal_string:
    STRING_LITERAL
    | STRING_BLOCK_LITERAL
    ;

literal:
    literal_integer 
    | literal_float
    | literal_bool
    | literal_char
    | literal_string
    ;

visibility_modifier:
    PUBLIC 
    ;

// ------------------------------------------------------------------------------------------------
// TOKENS
// ------------------------------------------------------------------------------------------------

STRUCT: 'struct';
INTERFACE: 'interface';
ENUM: 'enum';
UNION: 'union';
EXTENSION: 'extension';
REF: 'ref';
CONST: 'const';
FUNC: 'func';
VAR: 'var';
LET: 'let';
MODULE: 'module';
IMPORT: 'import';
CONSTRUCTOR: 'constructor';
FOR: 'for';
IN: 'in';
WHERE: 'where';
IF: 'if';
THEN: 'then';
ELSE: 'else';
WHILE: 'while';
IS: 'is';
NEW: 'new';
AS: 'as';
UNSAFE: 'unsafe';
UNIQUE: 'unique';
LIFETIME: 'lifetime';
TRY: 'try';
CATCH: 'catch';
THROWS: 'throws';
IGNORE: 'ignore';
EXCLUSIVE: 'exclusive';
STATIC: 'static';

CAN: 'can';
HAS: 'has';
ARE: 'are';

PUBLIC: 'public';
MUTABLE: 'mutable';
PARTIAL: 'partial';

IMPLEMENTS: 'implements';
EXTENDS: 'extends';
REQUIRES: 'requires';

GET: 'get';
SET: 'set';
THIS: 'this';

LEFT_SQUARE_BRACKET: '[';
RIGHT_SQUARE_BRACKET: ']';
LEFT_PAREN: '(';
RIGHT_PAREN: ')';
LEFT_BRACE: '{';
RIGHT_BRACE: '}';
COLON: ':';
COLON_COLON: '::';
EQUAL: '=';
STAR: '*';
TILDE: '~';
BANG: '!';
HASH: '#';
COMMA: ',';
GRAVE: '`';
LESS_THAN: '<';
LESS_THAN_OR_EQUAL: '<=';
GREATER_THAN: '>';
GREATER_THAN_OR_EQUAL: '>=';
MINUS_GREATER_THAN: '->';
EQUAL_GREATER_THAN: '=>';
DOT: '.';
PLUS: '+';
MINUS: '-';
DIVIDE: '/';
MODULO: '%';
EQUAL_EQUAL: '==';
BANGL_EQUAL: '!=';
BITWISE_AND: '&';
BITWISE_OR: '|';
BITWISE_XOR: '^';
AND: '&&';
OR: '||';
PLUS_EQUAL: '+=';
MINUS_EQUAL: '-=';
STAR_EQUAL: '*=';
DIVIDE_EQUAL: '/=';
BITWISE_AND_EQUAL: '&=';
BITWISE_OR_EQUAL: '|=';
BITWISE_XOR_EQUAL: '^=';
MODULO_EQUAL: '%=';

TYPE_BOOL:   'bool';

TYPE_U8:   'u8';
TYPE_U16:  'u16';
TYPE_U32:  'u32';
TYPE_U64:  'u64';
TYPE_U128: 'u128';

TYPE_I8:   'i8';
TYPE_I16:  'i16';
TYPE_I32:  'i32';
TYPE_I64:  'i64';
TYPE_I128: 'i128';

TYPE_F32:  'f32';
TYPE_F64:  'f64';

// Literals

DECIMAL_LITERAL:    ('0' | [1-9] (Digits? | '_'+ Digits));
HEX_LITERAL:        '0' [x] [0-9a-fA-F] ([0-9a-fA-F_]* [0-9a-fA-F])?;
OCT_LITERAL:        '0' '_'* [0-7] ([0-7_]* [0-7])?;
BINARY_LITERAL:     '0' [b] [01] ([01_]* [01])?;

FLOAT_LITERAL:      (Digits '.' Digits? | '.' Digits) ExponentPart?
             |       Digits ExponentPart
             ;

HEX_FLOAT_LITERAL:  '0' [x] (HexDigits '.'? | HexDigits? '.' HexDigits) [pP] [+-]? Digits;

BOOL_LITERAL:       'true'
            |       'false'
            ;

CHAR_LITERAL:       '\'' (~['\\\r\n] | EscapeSequence) '\'';
STRING_LITERAL:     '"' (~["\\\r\n] | EscapeSequence)* '"';
STRING_BLOCK_LITERAL:         '"""' [ \t]* [\r\n] (. | EscapeSequence)*? '"""';

// Whitespace and comments
WS:                 [ \t\r\n\u000C]+ -> channel(HIDDEN);
COMMENT:            '/*' .*? '*/'    -> channel(HIDDEN);
LINE_COMMENT:       '//' ~[\r\n]*    -> channel(HIDDEN);

IDENTIFIER: ([_]+ LetterOnly LetterOrDigit* | LetterOnly LetterOrDigit*);

UNDERSCORE: '_';

EOS: [\n;];

// Fragment rules

fragment ExponentPart
    : [eE] [+-]? Digits
    ;

fragment EscapeSequence
    : '\\' [btnfr"'\\]
    | '\\' ([0-3]? [0-7])? [0-7]
    | '\\' 'u'+ HexDigit HexDigit HexDigit HexDigit
    ;
fragment HexDigits
    : HexDigit ((HexDigit | '_')* HexDigit)?
    ;
fragment HexDigit
    : [0-9a-fA-F]
    ;
fragment Digits
    : [0-9] ([0-9_]* [0-9])?
    ;
fragment LetterOrDigit
    : Letter
    | [0-9]
    ;
fragment Letter
    : [a-zA-Z_]
    ;
fragment LetterOnly
    : [a-zA-Z]
    ;
