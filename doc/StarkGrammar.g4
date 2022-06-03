// ------------------------------------------------------------------
// Work in progress to define a new iteration on the grammar of stark
//
// NOTE: 
// The grammar is not fully functionnal for a lexer/parser as it is sensitive to NEWLINE 
// but doing that properly in ANTLR requires quite some custom hook in the grammar
// to handle them.
// ------------------------------------------------------------------

grammar StarkGrammar;

// ------------------------------------------------------------------
// Entrypoint
// ------------------------------------------------------------------    

file_declaration
    : module_declaration? 
      import_statement*
      top_level_declaration*
    ;

top_level_declaration
    : struct_declaration
    | union_declaration
    | enum_declaration
    | interface_declaration
    | extension_declaration
    | global_func_declaration
    | const_declaration
    | static_declaration 
    | attr_declaration
    | lifetime_declaration
    | unit_declaration
    | type_declaration
    ;

// ------------------------------------------------------------------
// Top level Declaration
// ------------------------------------------------------------------    

module_declaration
    : attr* visibility? 'partial'? 'module' module_path? identifier EOS
    ;

import_statement
    : 'import' module_path? import_identifier EOS
    ;

import_identifier
    : identifier
    | '*'
    | '{' identifier (',' identifier)* '}'
    ;

attr_declaration
    : attr* visibility? 'attr' identifier parameters EOS
    ;

attr
    : at_identifier arguments?
    ;

lifetime_declaration
    : attr* visibility? 'lifetime' lifetime (('<' | '<=') lifetime)+ EOS
    ;

const_declaration
    : attr* visibility? 'const' identifier (':' type)? '=' expression EOS
    ;

static_declaration
    : attr* 'static' identifier (':' type)? '=' expression EOS
    ;

struct_declaration
    : attr* visibility? 'partial'? 'type' 'ref'? 'mutable'? 'struct' identifier_with_generic_parameters parameters? struct_constraint* (EOS | '{' struct_members '}')
    ;

interface_declaration
    : attr* visibility? 'partial'? 'type' 'interface' identifier_with_generic_parameters parameters? interface_constraint* (EOS | '{' interface_members '}')
    ;

extension_declaration
    : attr* visibility? 'partial'? 'type' 'extension' generic_parameters? FOR qualified_type extension_constraint*  (EOS | '{' extension_members '}')
    ;

union_declaration
    : attr* visibility? 'type' 'union' identifier_with_generic_parameters union_constraint* '{' union_members '}'
    ;

enum_declaration
    : attr* visibility? 'type' 'enum' identifier (':' primitive_type)? '{' enum_members '}'
    ;

type_declaration
    : attr* visibility? 'type' 'indirect' identifier_with_generic_parameters where_constraint* '=' type EOS
    ;

union_constraint
    : where_constraint
    ;

where_constraint
    : 'where' (identifier | lifetime) ':' where_constraint_part (',' where_constraint_part)*
    ;

where_constraint_part
    : 'is' type
    | 'is' 'type' ('enum' | 'union' | 'struct' | 'interface' | 'unit')
    | 'can' 'new'
    | 'has' 'lifetime' ('<' | '<=') lifetime
    | 'has' 'constructor' identifier? parameters
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

implement_contraint
    : 'implements' qualified_type
    ;

extends_constraint
    : 'extends' qualified_type
    ;

struct_members
    : import_statement*
      const_declaration*
      field_declaration*
      constructor_declaration_with_visibility*
      func_member_declaration_with_visibility*
    ;    

union_members
    : union_member (',' union_member)* ','?
    ; 

union_member
    : identifier parameters
    | type
    ;

enum_members
    : enum_member (',' enum_member)* ','?
    ;

enum_member
    : identifier ('=' expression)
    ;

interface_members
    : import_statement*
      constructor_definition*
      func_member_declaration_with_visibility*
    ;

extension_members
    : import_statement*
      const_declaration*
      constructor_declaration_with_visibility*
      func_member_declaration_with_visibility*
    ;    

field_declaration
    : attr* visibility? ('let' | 'var') identifier ':' type EOS
    ;

unit_declaration
    : attr* visibility? 'unit' unit ('=' unit_expression)? EOS
    ;

unit_expression
    : unit ('^' literal_integer)?
    | literal_float
    | literal_integer
    | unit_expression ('/' '*') unit_expression
    ;

// ------------------------------------------------------------------
// Functions
// ------------------------------------------------------------------

func_member_declaration_with_visibility
    : attr* visibility? func_member_declaration
    ;    

global_func_declaration
    : attr* visibility? func_static_regular_declaration
    | attr* visibility? func_static_property_declaration
    ;        

func_member_declaration
    : func_this_regular_declaration
    | func_this_property_declaration
    | func_this_array_declaration
    | func_static_regular_declaration
    | func_static_property_declaration
    ;

func_this_regular_declaration
    : func_pre_modifier 'func' func_this func_regular_part
    ;

func_this_property_declaration
    : func_pre_modifier 'func' func_this func_property_part
    ;

func_this_array_declaration
    : func_pre_modifier 'func' func_this '[' identifier ':' type ']' func_return_type property_constraint* property_body
    ;

func_static_regular_declaration
    : func_pre_modifier 'func' 'static' func_regular_part
    ;

func_static_property_declaration
    : func_pre_modifier 'func' 'static' func_property_part
    ;    

func_regular_part
    : identifier_with_generic_parameters parameters func_return_type? throws_constraint? func_constraint* func_body
    ;

func_property_part
    : identifier_with_generic_parameters func_return_type property_constraint* property_body
    ;


func_pre_modifier
    : 'partial'? 'unsafe'?
    ;

func_this
    :  ('this' | 'mutable' 'this')
    ;

func_constraint
    : where_constraint
    | requires_constraint
    ;

throws_constraint
    : 'throws' qualified_type (',' qualified_type)*
    ;

property_constraint
    : requires_constraint
    ;

requires_constraint
    : 'requires' expression
    ;

func_global_modifier
    : 'partial'? 'unsafe'?
    ;

constructor_declaration_with_visibility
    : constructor_definition_with_visibility func_body
    ;

constructor_definition_with_visibility
    : attr* visibility? constructor_definition
    ;

constructor_definition
    : constructor_modifier 'func' 'constructor' identifier? parameters constructor_constraint*
    ;

constructor_constraint
    : requires_constraint
    ;

constructor_modifier
    : 'partial'? 'unsafe'?
    ;    

func_return_type
    : '->' attr* type
    ;

func_body
    : func_simple_body EOS
    | block_statement
    ;

property_body
    : func_simple_body
    | property_block_body
    ;

property_block_body
    : '{' property_getter property_setter? '}'
    ;

property_getter
    : 'get' func_body  // GET contextual only in property getter/setter
    ;
property_setter
    : 'set' func_body  // SET contextual only in property getter/setter
    ;

func_simple_body
    : '=>' expression
    ;

func_block_body
    : '{' 
      import_statement*  
      statement*
      '}'
    ;

// ------------------------------------------------------------------
// Shared
// ------------------------------------------------------------------

identifier_with_generic_parameters
    : identifier generic_parameters?
    ;

identifier_with_generic_arguments
    : identifier generic_arguments?
    ;

generic_parameters
    : '`' '<' generic_parameter (',' generic_parameter)* '>'
    ;

generic_arguments
    : '`' '<' generic_argument (',' generic_argument)* '>'
    ;

generic_parameter
    : generic_parameter_name ('=' generic_argument)?
    | lifetime
    ;

generic_argument
    : generic_argument_type
    | generic_argument_literal
    | lifetime
    ;

generic_argument_literal
    : literal
    ;

generic_argument_type
    : type
    ;

generic_parameter_name
    : identifier
    ;

lifetime:
    HASH_IDENTIFIER
    ;

qualified_type
    : module_path? identifier_with_generic_arguments 
    ;

at_identifier
    : AT_IDENTIFIER
    ;

identifier
    : IDENTIFIER
    | 'struct'
    | 'attr'
    | 'interface'
    | 'enum'
    | 'union'
    | 'extension'
    | 'module'
    | 'constructor'
    | 'in'
    | 'where'
    | 'lifetime'
    | 'exclusive'
    | 'static'
    | 'unit'
    | 'indirect'
    | 'partial'
    | 'implements'
    | 'extends'
    | 'requires'
    | 'can'
    | 'has'
    | 'are'
    | 'get'
    | 'set'
    ;

module_path
    : (identifier '::')+
    ;

parameters
    : '(' parameter_list? ')'
    ;

parameter_list
    : parameter parameter_cont*
    ;

parameter_cont
    : ',' parameter
    ;

parameter
    : parameter_name ':' attr* type
    ;

parameter_name
    : IDENTIFIER
    ;

visibility
    : 'public' 
    ;

// ------------------------------------------------------------------
// Statements
// ------------------------------------------------------------------

statement:
    let_var_statement
    | for_statement
    | if_statement
    | while_statement
    | unsafe_statement
    | block_statement
    | expression_statement
    | assign_statement
    ;

let_var_statement:
    attr* ('let' | 'var') identifier ':' type ('=' expression) EOS
    ;

for_statement:
    attr* 'for' identifier 'in' expression block_statement
    ;

if_statement:
    attr* 'if' expression 'then' block_statement ('else' 'if' expression 'then' block_statement)* ('else' block_statement)?
    ;

while_statement:
    attr* 'while' expression block_statement
    ;

block_statement
    : attr* '{' statement* '}'
    ;

unsafe_statement:
    'unsafe' block_statement
    ;

expression_statement
    : expression_simple EOS
    ;

assign_statement:
    left_expression ('=' | '+=' | '-=' | '*=' | '/=' | '&=' | '|=' | '^=' | '<' '<' '=' |  '>' '>' '=' | '%=' ) expression EOS
    ;

// ------------------------------------------------------------------
// Expressions
// ------------------------------------------------------------------

expression
    : expression_simple
    | 'if' expression_simple 'then' expression_simple 'else' expression_simple
    | 'unsafe' expression_simple
    ;

// TODO: order is still not fully correct for operator precedence
expression_simple
    : '(' expression_list ')'
    | 'this'
    | literal
    // left_expression
    | expression_simple expression_path
    | module_path? (identifier | method_call)
    | module_path? identifier_with_generic_arguments '.' (identifier | method_call)
    // end of left_expression
    | '.' (identifier | method_call)
    | new_expression
    | prefix=('+'|'-') expression_simple
    | prefix=('~'|'!') expression_simple
    | expression_simple bop='as' expression_simple
    | expression_simple unit
    | expression_simple bop=('*'|'/'|'%') expression_simple
    | expression_simple bop=('+'|'-') expression_simple
    | expression_simple ('<' '<' | '>' '>') expression_simple
    | expression_simple bop=('<=' | '>=' | '<' | '>') expression_simple
    | expression_simple bop=IS type
    | expression_simple bop=('==' | '!=') expression_simple
    | expression_simple bop='&' expression_simple
    | expression_simple bop='^' expression_simple
    | expression_simple bop='|' expression_simple
    | expression_simple bop='&&' expression_simple
    | expression_simple bop='||' expression_simple
    | 'ref' expression_simple
    | '&' expression_simple
    | 'catch'? 'try' expression_simple
    | 'ignore' expression_simple
    ;

left_expression
    : module_path? (identifier | method_call)
    | literal expression_path
    | 'this' expression_path
    | '(' expression_list ')' expression_path
    | left_expression expression_path
    ;

expression_path
    : '.' identifier
    | '.' method_call
    | '[' expression ']'
    ;

new_expression:
    'new' lifetime? type_constructor
    ;

type_constructor:
    module_path? identifier_with_generic_arguments arguments
    // TODO: Add array constructor
    ;    

method_call:
    identifier_with_generic_arguments arguments
    ;

expression_list:
    expression (',' expression)*
    ;

arguments:
    '(' argument_list? ')'
    ;

argument_list:
    expression (',' expression)*
    ;

// ------------------------------------------------------------------
// Types
// ------------------------------------------------------------------

type:
    primitive_type
    | qualified_type
    | ref_type
    | mutable_type
    | array_type
    | fixed_array_type
    | slice_type
    | pointer_type
    | const_type
    | measure_type
    ;

primitive_type
    : bool_type
    | integer_type
    | float_type
    | vector_type
    ;

bool_type
    : 'bool'
    ;

integer_type
    : 'uint'
    | 'u8'
    | 'u16'
    | 'u32'
    | 'u64'
    | 'int'
    | 'i8'
    | 'i16'
    | 'i32'
    | 'i64'
    ;

float_type
    : 'f32'
    | 'f64'
    ;

vector_type
    : 'v128'
    | 'v256'
    ;    

const_type:
    'const' type
    ;

ref_type:
    'unique'? 'ref' lifetime? type
    ;

mutable_type:
    'mutable' type
    ;

array_type:
    '[' type ']' 
    ;

// We will validate the <expression> for the dimension in the semantic analysis
fixed_array_type:
    '[' type ',' expression ']' 
    ;    

slice_type:
    '~' type
    ;

pointer_type:
    '*' type
    ;

unit
    : identifier
    ;

measure_type:
    (integer_type | float_type) '*' unit
    ;

// ------------------------------------------------------------------
// Literals
// ------------------------------------------------------------------

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


// ------------------------------------------------------------------
// KEYWORDS
// ------------------------------------------------------------------

TYPE: 'type';
FUNC: 'func';
VAR: 'var';
LET: 'let';
REF: 'ref';
CONST: 'const';
IMPORT: 'import';
FOR: 'for';
IF: 'if';
THEN: 'then';
ELSE: 'else';
WHILE: 'while';
IS: 'is';
NEW: 'new';
AS: 'as';
UNIQUE: 'unique';
TRY: 'try';
CATCH: 'catch';
THROWS: 'throws';
IGNORE: 'ignore';
PUBLIC: 'public';
MUTABLE: 'mutable';
THIS: 'this';
UNSAFE: 'unsafe';
STATIC: 'static';

// ------------------------------------------------------------------
// NON KEYWORDS
// ------------------------------------------------------------------

STRUCT: 'struct';
ATTR: 'attr';
INTERFACE: 'interface';
ENUM: 'enum';
UNION: 'union';
EXTENSION: 'extension';
MODULE: 'module';
CONSTRUCTOR: 'constructor';
IN: 'in';
WHERE: 'where';
LIFETIME: 'lifetime';
EXCLUSIVE: 'exclusive';
UNIT: 'unit';
INDIRECT: 'indirect';
PARTIAL: 'partial';
IMPLEMENTS: 'implements';
EXTENDS: 'extends';
REQUIRES: 'requires';
CAN: 'can';
HAS: 'has';
ARE: 'are';
GET: 'get';
SET: 'set';

LEFT_SQUARE_BRACKET: '[';
RIGHT_SQUARE_BRACKET: ']';
LEFT_PAREN: '(';
RIGHT_PAREN: ')';
LEFT_BRACE: '{';
RIGHT_BRACE: '}';
COLON_COLON: '::';
COLON: ':';
STAR: '*';
TILDE: '~';
BANG: '!';
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
BANG_EQUAL: '!=';
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
EQUAL: '=';

TYPE_BOOL: 'bool';

TYPE_UINT: 'uint';
TYPE_U8:   'u8';
TYPE_U16:  'u16';
TYPE_U32:  'u32';
TYPE_U64:  'u64';

TYPE_INT:  'int';
TYPE_I8:   'i8';
TYPE_I16:  'i16';
TYPE_I32:  'i32';
TYPE_I64:  'i64';

TYPE_F32:  'f32';
TYPE_F64:  'f64';

TYPE_V128: 'v128';
TYPE_V256: 'v256';

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

HASH_IDENTIFIER: '#' IDENTIFIER;
HASH: '#';

AT_IDENTIFIER: '@' IDENTIFIER;
AT: '@';

// This is not fully correct, that should not be hidden, 
// but we don't want to handle correctly NEW_LINE in this grammar
EOS: [\n;] -> channel(HIDDEN);

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
