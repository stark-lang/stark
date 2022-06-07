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
// Compilation Unit
// ------------------------------------------------------------------    

compilation_unit
    // Expected to be in this order, we will warning otherwise
    : module_declaration? 
      statement_import*
      module_member_declaration*
    ;

module_member_declaration
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
    | alias_type_declaration
    | alias_func_declaration
    | macro_declaration
    ;

// ------------------------------------------------------------------
// Top level Declaration
// ------------------------------------------------------------------    

module_declaration
    : attr* visibility? 'partial'? 'module' module_path? identifier EOS
    ;

statement_import
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
    : attr* visibility? 'partial'? 'ref'? 'mutable'? 'struct' identifier_with_generic_parameters parameters? implement_contraint* where_constraint* (EOS | '{' struct_members '}')
    ;

interface_declaration
    : attr* visibility? 'partial'? 'interface' identifier_with_generic_parameters parameters? extends_constraint* where_constraint* (EOS | '{' interface_members '}')
    ;

extension_declaration
    : attr* visibility? 'partial'? 'extension' generic_parameters? FOR qualified_type implement_contraint* where_constraint* (EOS | '{' extension_members '}')
    ;

union_declaration
    : attr* visibility? 'union' identifier_with_generic_parameters where_constraint* '{' union_members '}'
    ;

enum_declaration
    : attr* visibility? 'enum' identifier (':' primitive_type)? '{' enum_members '}'
    ;

type_declaration
    : attr* visibility? 'type' identifier_with_generic_parameters where_constraint* '=' core_type EOS
    ;

alias_type_declaration
    : attr* visibility? 'alias' 'type' identifier '=' core_type EOS
    ;

alias_func_declaration
    : attr* visibility? 'alias' 'func' identifier '=' qualified_name (DOT identifier)? EOS
    ;

where_constraint
    : 'where' lifetime (',' lifetime)* ':' where_constraint_lifetime_part (',' where_constraint_lifetime_part)*
    | 'where' identifier (',' identifier)* ':' where_constraint_part (',' where_constraint_part)*
    ;

where_constraint_part
    : 'is' type
    | 'kind' ('enum' | 'union' | 'struct' | 'interface' | 'unit' | 'ref' 'struct') // TODO: should we add e.g | 'integer' | 'float' | 'number'?
    | 'has' 'constructor' identifier? parameters
    ;

where_constraint_lifetime_part
    : 'has' 'lifetime' ('<' | '<=') lifetime
    | 'can' 'new'
    ;

implement_contraint
    : 'implements' qualified_type
    ;

extends_constraint
    : 'extends' qualified_type
    ;

struct_members
    // Expected to be in this order, we will warning otherwise
    : statement_import*
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
    // Expected to be in this order, we will warning otherwise
    : (attr* constructor_definition)*
      (attr* func_member_declaration)*
    ;

extension_members
    // Expected to be in this order, we will warning otherwise
    : statement_import*
      const_declaration*
      constructor_declaration_with_visibility*
      func_member_declaration_with_visibility*
    ;    

field_declaration
    : attr* visibility? ('let' | 'var') identifier ':' type EOS
    ;

unit_declaration
    : attr* visibility? 'unit' identifier ('=' unit_expression)? EOS
    ;

unit_expression
    : identifier ('^' literal_integer)?
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
    : attr* visibility? func_static_regular_declaration_part
    | attr* visibility? func_static_property_declaration_part
    ;        

func_member_declaration
    : func_this_regular_declaration_part
    | func_this_property_declaration_part
    | func_this_array_declaration_part
    | func_static_regular_declaration_part
    | func_static_property_declaration_part
    ;

func_this_regular_declaration_part
    : func_pre_modifier* 'func' func_this func_regular_part
    ;

func_this_property_declaration_part
    : func_pre_modifier* 'func' func_this func_property_part
    ;

func_this_array_declaration_part
    : func_pre_modifier* 'func' func_this '[' identifier ':' type ']' func_return_type property_constraint* property_body
    ;

func_static_regular_declaration_part
    : func_pre_modifier* 'func' func_regular_part
    ;

func_static_property_declaration_part
    : func_pre_modifier* 'func' func_property_part
    ;    

func_regular_part
    : identifier_with_generic_parameters parameters func_return_type? throws_constraint? func_constraint* func_body
    ;

func_property_part
    : identifier_with_generic_parameters func_return_type property_constraint* property_body
    ;

func_pre_modifier
    : 'partial'
    | 'unsafe'
    | 'dispatch' // only valid for regular functions
    | async
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
    : constructor_modifier 'constructor' identifier? parameters constructor_constraint*
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
    : func_expression_body EOS
    | expression_block
    ;

property_body
    : func_expression_body EOS
    | property_block_body
    ;

property_block_body
    // Expected to be in this order, we will warning otherwise
    : '{' property_getter property_setter? '}'
    ;

property_getter
    : 'get' func_body  // GET contextual only in property getter/setter
    ;
property_setter
    : 'set' func_body  // SET contextual only in property getter/setter
    ;

func_expression_body
    : '=>' expression
    ;

func_block_body
    : expression_block
    ;

// parametrize async/await
async
    : 'async'
    | 'async' '`' async_await_generic_short_param
    | 'async' '`' '<' async_await_generic_long_param '>'
    ;

await
    : 'await'
    | 'await' '`' async_await_generic_short_param
    | 'await' '`' '<' async_await_generic_long_param '>'
    ;

async_await_generic_short_param
    : identifier
    | literal_bool
    ;

async_await_generic_long_param
    : const_path
    | literal_bool
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
    | '`' (identifier | lifetime | literal_integer | literal_bool | primitive_type) // for small one arg generic (e.g no module path)
    ;

generic_parameter
    : identifier ('=' generic_argument)?
    | lifetime
    ;

generic_argument
    : type
    | literal
    | lifetime
    ;

lifetime
    : HASH_IDENTIFIER
    ;

qualified_func
    : qualified_name
    ;

qualified_name
    : module_path? identifier_with_generic_arguments 
    ;

qualified_name_no_generic_arguments
    : module_path? identifier
    ;

at_identifier
    : AT_IDENTIFIER
    ;

identifier
    : macro_inline_call
    | IDENTIFIER
    | 'alias'
    | 'are'
    | 'attr'
    | 'can'
    | 'case'
    | 'constructor'
    | 'enum'
    | 'exclusive'
    | 'extends'
    | 'extension'
    | 'get'
    | 'has'
    | 'implements'
    | 'import'
    | 'in'
    | 'indirect'
    | 'interface'
    | 'kind'
    | 'lifetime'
    | 'module'
    | 'macro'
    | 'partial'
    | 'requires'
    | 'set'
    | 'static'
    | 'struct'
    | 'throws'
    | 'type'
    | 'union'
    | 'unit'
    | 'where'
    // Macro tokens
    | 'identifier'
    | 'lifetime'
    | 'expression'
    | 'statement'
    | 'literal'
    | 'token'
    ;

module_path
    : (identifier ':' ':')+
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
    | '.' '.' '.' // variable parameter
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

statement
    : attr* 
        ( statement_let_var
        | statement_for
        | statement_while
        | statement_continue
        | statement_break
        | statement_return
        | statement_expression
        )
    ;

statement_let_var
    : ('let' | 'var') identifier ':' type ('=' expression) EOS
    ;

statement_for
    : (async | await)? 'for' identifier 'in' expression expression_block
    ;

statement_while
    : 'while' expression expression_block
    ;

statement_return
    : 'return' expression? EOS
    ;

statement_continue
    : 'continue' EOS
    ;

statement_break
    : 'break' EOS
    ;

statement_expression
    : expression_statement EOS
    ;

// ------------------------------------------------------------------
// Expressions
// ------------------------------------------------------------------

expression_statement
    : expression
    // left ':=' right is a deref and assign equivalent to: *left = right
    | expression_assignable (':=' | '=' | '+=' | '-=' | '*=' | '/=' | '&=' | '|=' | '^=' | '<' '<' '=' |  '>' '>' '=' | '%=' ) expression EOS
    ;

expression
    : 'unsafe'? (expression_with_block | expression_without_block)
    ;

expression_with_block
    : expression_block
    | expression_if_with_block
    ;

// If an if expression starts to use expression_block, it should use them all along
expression_if_with_block
    : 'if' expression_unary_or_binary 'then' expression_block ('else' expression_with_block)?
    ;

expression_without_block
    : expression_unary_or_binary
    | ('ref' | '&') expression_unary_or_binary
    | expression_if
    | expression_match
    | expression_range
    | expression_anonymous_func
    | expression_constructor_struct
    | expression_constructor_array
    ;

expression_range
    : expression_unary_or_binary? '.' '.' expression_unary_or_binary?
    ;

// If an if expression starts to use expression_without_block, it should use them all along
expression_if
    : 'if' expression_unary_or_binary 'then' expression_without_block ('else' expression_without_block)+
    ;

expression_match
    : 'match' expression_unary_or_binary '{' expression_case (',' expression_case)* ','? '}'
    ;

expression_case
    : 'case' expression_pattern func_expression_body
    ;

expression_pattern
    // case _ => ...
    : expression_discard
    // case x: int => ...
    | (identifier ':')? type
    // case 0, 1, 2 => ...
    | literal (',' literal)*
    // case .ENUM_VALUE1, .ENUM_VALUE2 => ...
    | '.' identifier (',' '.' identifier)*
    // TODO add more patterns (range, ...etc.)
    ;

expression_block
    : '{' statement* '}'
    ;

expression_unary_or_binary
    : expression_unary
    | expression_unary_or_binary measure_type // implicitly convert to *, so same precedence
    | expression_unary_or_binary bop=('*'|'/'|'%') expression_unary_or_binary
    | expression_unary_or_binary bop=('+'|'-') expression_unary_or_binary
    | expression_unary_or_binary bop='as' type
    | expression_unary_or_binary bop='is' (identifier ':')? type 
    | expression_unary_or_binary bop='is' 'not' type 
    | expression_unary_or_binary ('<' '<' | '>' '>') expression_unary_or_binary
    | expression_unary_or_binary bop=('<=' | '>=' | '<' | '>') expression_unary_or_binary
    | expression_unary_or_binary bop=('==' | '!=') expression_unary_or_binary
    | expression_unary_or_binary bop='&' expression_unary_or_binary
    | expression_unary_or_binary bop='^' expression_unary_or_binary
    | expression_unary_or_binary bop='|' expression_unary_or_binary
    | expression_unary_or_binary bop='&&' expression_unary_or_binary
    | expression_unary_or_binary bop='||' expression_unary_or_binary
    ;

expression_unary
    : expression_primary1 expression_member_path*
    | expression_primary2 expression_member_path*
    | expression_new
    | 'throw' expression_unary_or_binary
    | 'catch'? 'try' expression_unary_or_binary
    | async expression_unary_or_binary
    | await expression_unary_or_binary    
    | prefix=('+'|'-') expression_unary_or_binary
    | prefix=('~'|'!') expression_unary_or_binary
    ;

expression_anonymous_func
    : (async | await)? 'func' generic_arguments? arguments? func_return_type? func_expression_body
    | (async | await)? identifier func_expression_body
    | (async | await)? '(' identifier (',' identifier)* ')' func_expression_body
    ;

expression_assignable
    : expression_primary1 expression_member_path+ // assignable expressions that requires at least one member path
    | expression_primary2 expression_member_path* // assignable expressions that can be assigned directly
    ;

// These expressions cannot be assigned directly
expression_primary1
    : 'this'                    #expression_this
    | literal                   #expression_literal
    // An enum or union constructor
    | '.' (identifier | method_call) #expression_enum_or_union
    ;

// These expressions can be assigned directly
expression_primary2
    // A local variable: e.g x
    // A local func call: e.g f(x)
    // A module variable: e.g core::sub::x
    // A module func call: e.g core::sub::f(x)
    : module_path? (identifier | method_call)
    // A field of a generic type, e.g:
    // - MyStruct`<int>.x / MyStruct`<int>.f(x)
    // - core::sub::MyStruct`<int>.x / core::sub::MyStruct`<int>.f(x)
    | module_path? identifier generic_arguments expression_dot_path
    // Group and Tuple/List expression
    | '(' expression (',' expression)* ')'
    // Dereferencing
    | '*' expression_unary_or_binary
    // Discard '_'
    | expression_discard
    ;

expression_member_path
    : expression_dot_path
    | expression_indexer_path
    ;

expression_dot_path
    : '.' identifier
    | '.' method_call
    ;

expression_indexer_path
    : '[' expression ']'
    ;

// A const expression path is an identifier, or a path to a const
// that could involve a generic instance type
const_path
    : qualified_name_no_generic_arguments // path for a const declared in a module: e.g core::module::const_field
    | qualified_name '.' identifier // generic instance const field: core::module::MyType<true>.const_field
    ;

expression_new
    : 'new' lifetime? expression_new_constructor
    ;

expression_new_constructor
    : expression_constructor_struct
    | expression_constructor_array
    | expression_constructor_string
    ; 


expression_constructor_struct
    : module_path? identifier_with_generic_arguments arguments? list_initializer?
    ;

expression_constructor_array
    : '[' type ']' list_initializer?                    // with list initializer
    | '[' type ']' '(' expression ')' list_initializer? // with list initializer
    | '[' type ']' '(' expression ',' expression ')'    // with func initializer
    | '[' 'u8' ']' literal_string                       // with list initializer
    ;

expression_constructor_string
    : literal_string
    ;

expression_discard
    : '_'
    ;

list_initializer
    : '{' expression (',' expression)* '}'
    ;  

method_call
    : identifier_with_generic_arguments arguments
    ;

expression_list
    : expression (',' expression)*
    ;

arguments
    : '(' expression_list? ')'
    ;

// ------------------------------------------------------------------
// Macros
// ------------------------------------------------------------------

macro_declaration
    : visibility? ('partial' | 'extern')? 'macro' identifier '{' macro_members '}'
    ;

macro_members
    : attr
    | macro_func_declaration
    ;

macro_func_declaration
    : 'func' '(' macro_func_parameter* ')' macro_func_body
    | 'func' '[' macro_func_parameter* ']' macro_func_body
    | 'func' '{' macro_func_parameter* '}' macro_func_body
    ;

macro_func_parameter
    : macro_identifier ':' macro_func_parameter_expression*
    ;

macro_func_parameter_expression
    : 'identifier'
    | 'lifetime'
    | 'expression'
    | 'statement'
    | 'literal'
    | 'token'
    | macro_literal_token
    | '?' macro_func_parameter_expression
    | '*' macro_func_parameter_expression
    | '+' macro_func_parameter_expression
    | '(' macro_func_parameter_expression+ ')'
    ;

macro_func_body
    : '=>' macro_statement EOS
    // here the number of leading $ must match the trailing $
    // but we can't express here this without custom hooks
    // so we declare only 3 levels
    | '{' '$' macro_statement* '$' '}'
    | '{' '$' '$' macro_statement* '$' '$' '}'
    | '{' '$' '$' '$' macro_statement* '$' '$' '$' '}'
    ;

macro_statement
    : macro_tokens
    | '(' macro_statement* ')'
    | '[' macro_statement* ']'
    | '{' macro_statement* '}'
    // here the number of leading $ must match the trailing $
    // but we can't express here this without custom hooks
    // so we declare only 3 levels
    | '<' '$' macro_command '$' '>'
    | '<' '$' '$' macro_command '$' '$' '>'
    | '<' '$' '$' '$' macro_command '$' '$' '$' '>'
    ;

macro_command
    : 'for' macro_identifier 'in' macro_identifier '{'
    | 'if' macro_expression 'then' '{'
    | '}' 'else' '{'
    | 'else'
    | '{'
    | '}'
    ;

// All tokens except group tokens '('|')'|'['|']'|'{'|'}'
// as we expect that all tokens are well balanced according to group tokens.
macro_tokens
    :'_'|'-'|'-='|'->'|','|'::'|':'|':='|'!'|'!='|'.'|'@'|'*'|'*='|'/'|'/='|'&'|'&&'|'&='|'#'|'%'|'%='|'`'|'^'|'^='|'+'|'+='|'<'|'<='|'='|'=='|'=>'|'>'|'>='|'|'|'|='|'||'|'~'|'alias'|'are'|'as'|'async'|'attr'|'await'|'bool'|'break'|'can'|'catch'|'const'|'constructor'|'continue'|'else'|'enum'|'exclusive'|'expression'|'extends'|'extension'|'f32'|'f64'|'for'|'func'|'get'|'has'|'i16'|'i32'|'i64'|'i8'|'identifier'|'if'|'ignore'|'implements'|'import'|'in'|'indirect'|'int'|'interface'|'is'|'kind'|'let'|'lifetime'|'lifetime'|'literal'|'macro'|'module'|'mutable'|'new'|'not'|'partial'|'public'|'ref'|'requires'|'return'|'set'|'statement'|'static'|'struct'|'then'|'this'|'throw'|'throws'|'token'|'try'|'type'|'u16'|'u32'|'u64'|'u8'|'uint'|'union'|'unique'|'unit'|'unsafe'|'v128'|'v256'|'var'|'where'|'while'
    | literal
    | IDENTIFIER
    | lifetime
    | at_identifier
    | macro_identifier
    | macro_literal_token
    ;

macro_expression
    : macro_identifier
    | literal
    | '(' macro_expression ')'
    | macro_expression bop=('<=' | '>=' | '<' | '>') macro_expression
    | macro_expression bop=('==' | '!=') macro_expression
    | macro_expression bop='&&' macro_expression
    | macro_expression bop='||' macro_expression    
    ;

macro_identifier
    : MACRO_IDENTIFIER
    ;

macro_literal_token
    : MACRO_STRING_LITERAL
    ;

macro_inline_call
    : macro_identifier '(' macro_argument* ')'
    | macro_identifier '[' macro_argument* ']'
    | macro_identifier '{' macro_argument* '}'
    ;

macro_argument
    : macro_tokens
    | '(' macro_argument* ')'
    | '[' macro_argument* ']'
    | '{' macro_argument* '}'
    ;

// ------------------------------------------------------------------
// Types
// ------------------------------------------------------------------

type
    : non_const_type
    | const_type
    ;

non_const_type
    : core_type
    | ref_type
    | mutable_type
    | pointer_type
    ;

// Core types are types without a qualifier (ref, const, mutable, pointer)
// Usable by e.g type_declaration
core_type
    : mutable_core_type 
    | func_type // TODO: should it be a core_type or not?
    ;

mutable_core_type
    : primitive_type 
    | qualified_type
    | array_type
    | fixed_array_type
    | slice_type
    | tuple_type
    | measure_type
    | union_type
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

qualified_type
    : qualified_name
    ;

const_type
    : 'const' non_const_type // avoid const const
    ;

ref_type
    : 'unique'? 'ref' lifetime? type
    ;

tuple_type
    : '(' type (',' type)+ ')'
    ;

mutable_type
    : 'mutable' mutable_core_type
    ;

union_type
    : '.' identifier
    ;

array_type
    : '[' type ']' 
    ;

// We will validate the <expression> for the dimension in the semantic analysis
fixed_array_type
    : '[' type ',' expression ']' 
    ;    

slice_type
    : '~' type // should we use % vs ~ for a slice?
    ;

pointer_type
    : '*' (primitive_type | qualified_type)
    ;

unit
    : '`' identifier
    ;

measure_type
    : (integer_type | float_type) unit
    ;

func_type
    : async? 'func' generic_parameters? parameters func_return_type? throws_constraint? func_constraint*
    ;

// ------------------------------------------------------------------
// Literals
// ------------------------------------------------------------------

literal_integer
    : DECIMAL_LITERAL | HEX_LITERAL | OCT_LITERAL | BINARY_LITERAL
    ;

literal_float
    : FLOAT_LITERAL
    ;

literal_bool
    : BOOL_LITERAL
    ;

literal_char
    : CHAR_LITERAL
    ;

literal_string
    : STRING_LITERAL
    | STRING_BLOCK_LITERAL
    ;

literal
    : literal_integer integer_type?
    | literal_float float_type?
    | literal_bool
    | literal_char
    | literal_string
    ;

// ------------------------------------------------------------------
// KEYWORDS
// ------------------------------------------------------------------

AS: 'as';
ASYNC: 'async';
AWAIT: 'await';
BREAK: 'break';
CATCH: 'catch';
CONST: 'const';
CONTINUE: 'continue';
ELSE: 'else';
FOR: 'for';
FUNC: 'func';
IF: 'if';
IS: 'is';
LET: 'let';
MATCH: 'match';
MUTABLE: 'mutable';
NEW: 'new';
NOT: 'not';
PUBLIC: 'public';
REF: 'ref';
RETURN: 'return';
THEN: 'then';
THIS: 'this';
THROW: 'throw';
TRY: 'try';
UNIQUE: 'unique';
UNSAFE: 'unsafe';
VAR: 'var';
WHILE: 'while';

// ------------------------------------------------------------------
// NON KEYWORDS (can be used as identifiers)
// ------------------------------------------------------------------

ALIAS: 'alias';
ARE: 'are';
ATTR: 'attr';
CAN: 'can';
CONSTRUCTOR: 'constructor';
DISPATCH: 'dispatch';
ENUM: 'enum';
EXCLUSIVE: 'exclusive';
EXTENDS: 'extends';
EXTENSION: 'extension';
GET: 'get';
HAS: 'has';
IMPLEMENTS: 'implements';
IMPORT: 'import';
IN: 'in';
INDIRECT: 'indirect';
INTERFACE: 'interface';
KIND: 'kind';
LIFETIME: 'lifetime';
MODULE: 'module';
MACRO: 'macro';
PARTIAL: 'partial';
REQUIRES: 'requires';
SET: 'set';
STATIC: 'static';
STRUCT: 'struct';
THROWS: 'throws';
TYPE: 'type';
UNION: 'union';
UNIT: 'unit';
WHERE: 'where';

// Begin MACRO Tokens
MACRO_TOKEN_IDENTIFIER: 'identifier';
MACRO_TOKEN_EXPRESSION: 'expression';
MACRO_TOKEN_STATEMENT: 'statement';
MACRO_TOKEN_LITERAL: 'literal';
MACRO_TOKEN_TOKEN: 'token';
// End MACRO Tokens

LEFT_SQUARE_BRACKET: '[';
RIGHT_SQUARE_BRACKET: ']';
LEFT_PAREN: '(';
RIGHT_PAREN: ')';
LEFT_BRACE: '{';
RIGHT_BRACE: '}';
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
COLON_EQUAL: ':=';

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

CHAR_LITERAL:         '\'' (~['\\\r\n] | EscapeSequence) '\'';
MACRO_STRING_LITERAL: '\'' (~['\\])* '\'';

// Interpolated strings are not correctly described here, but as it requires parser
// cooperation, we will leave it to the implementation.
STRING_LITERAL:       '$'? '"' (~["\\\r\n] | EscapeSequence)* '"';

// Block literal is not correctly described here. 
// The opening """ is actually at least 3+, and the closing should match the opening
STRING_BLOCK_LITERAL: '$'? '"""' .*? '"""';

// Whitespace and comments
WS:                 [ \t\r\n\u000C]+ -> channel(HIDDEN);
COMMENT:            '/*' .*? '*/'    -> channel(HIDDEN);
LINE_COMMENT:       '//' ~[\r\n]*    -> channel(HIDDEN);

IDENTIFIER: Identifier;
UNDERSCORE: '_';

HASH_IDENTIFIER: '#' Identifier;
HASH: '#';

AT_IDENTIFIER: '@' Identifier;
AT: '@';

MACRO_IDENTIFIER: '$'+ Identifier;
DOLLAR: '$';

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
fragment LetterOrDigitOrUnderscore
    : LetterOrDigit
    | '_'
    ;

fragment LetterOrDigit
    : Letter
    | [0-9]
    ;    

fragment Letter
    : [a-zA-Z]
    ;

fragment Identifier
    : [_]+ LetterOrDigit LetterOrDigitOrUnderscore*
    | Letter LetterOrDigitOrUnderscore*
    ;
