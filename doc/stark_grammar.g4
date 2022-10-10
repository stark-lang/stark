// ------------------------------------------------------------------
// Work in progress to define a new iteration on the grammar of stark
//
// NOTE: 
// The grammar is not fully functionnal for a lexer/parser as it is sensitive to NEWLINE 
// but doing that properly in ANTLR requires quite some custom hook in the grammar
// to handle them.
// ------------------------------------------------------------------

grammar stark_grammar;

/**
Compilation Unit
*/
compilation_unit
    : decl_compilation_unit_member* 
    ;

decl_compilation_unit_member
    : decl_module
    | stmt_import
    | decl_module_member
    ;

decl_module_member
    : decl_struct
    | decl_union
    | decl_enum
    | decl_interface
    | decl_extend
    | decl_global_func
    | decl_const
    | decl_static 
    | decl_attr
    | decl_unit
    | decl_type
    | decl_alias_type
    | decl_alias_func
    | decl_macro
    | macro_inline_call
    ;

// ------------------------------------------------------------------
// Top level Declaration
// ------------------------------------------------------------------    

decl_module
    : pre_decl 'partial'? 'module' module_path* identifier EOS
    ;

stmt_import
    : 'import' module_path* import_identifier EOS
    ;

import_identifier
    : identifier                #import_identifier_single
    | '*'                       #import_identifier_wildcard
    | '{' identifier_list '}'   #import_identifier_list
    ;

decl_attr
    : pre_decl 'attr' identifier parameters EOS
    ;

attr
    : '@' identifier attr_arguments?
    ;

decl_const
    : pre_decl 'const' identifier (':' type)? '=' expr EOS
    ;

decl_static
    : pre_decl 'static' identifier (':' type)? '=' expr EOS
    ;

decl_struct
    : pre_decl 'partial'? 'rw'? 'struct' identifier layout_parameters? generic_parameters? parameters? implement_contraint* where_constraint* (EOS | '{' struct_members '}')
    ;

decl_interface
    : pre_decl 'partial'? 'interface' identifier_with_generic_parameters parameters? inherits_constraint* where_constraint* (EOS | '{' interface_members '}')
    ;

decl_extend
    : pre_decl 'extend' generic_parameters? FOR type_qualified implement_contraint* where_constraint* (EOS | '{' extend_members '}')
    ;

decl_union
    : pre_decl 'union' identifier_with_generic_parameters where_constraint* '{' union_members '}'
    ;

decl_enum
    : pre_decl 'enum' identifier (':' type_primitive)? '{' enum_members '}'
    ;

decl_type
    : pre_decl 'type' identifier_with_generic_parameters where_constraint* '=' type_core EOS
    ;

decl_alias_type
    : pre_decl 'alias' 'type' identifier '=' type_core EOS
    ;

decl_alias_func
    : pre_decl 'alias' 'func' identifier '=' qualified_name (DOT identifier)? EOS
    ;

pre_decl
    : attr* visibility?
    ;

where_constraint
    : 'where' lifetime (',' lifetime)* ':' where_constraint_lifetime_part (',' where_constraint_lifetime_part)*
    | 'where' identifier (',' identifier)* ':' where_constraint_part (',' where_constraint_part)*
    ;

where_constraint_part
    : 'is' type
    | 'kind' ('enum' | 'union' | 'rw'? 'struct' | 'interface' | 'unit' | 'layout' | 'ownership' | 'permission') // TODO: should we add e.g | 'integer' | 'float' | 'number'?
    | 'has' 'constructor' identifier? parameters
    ;

where_constraint_lifetime_part
    : 'has' 'lifetime' ('<' | '<=') lifetime
    | 'can' 'new'
    ;

implement_contraint
    : 'implements' type_qualified
    ;

inherits_constraint
    : 'inherits' type_qualified
    ;

struct_members
    // Expected to be in this order, we will warning otherwise
    : stmt_import*
    | decl_const*
    | decl_field*
    | constructor_decl_with_visibility*
    | func_member_decl_with_visibility*
    | operator_member_decl_with_visibility*
    | macro_inline_call*
    ;    

union_members
    : union_member (',' union_member)* ','?
    ; 

union_member
    : '.' identifier parameters?
    | type
    ;

enum_members
    : enum_member (',' enum_member)* ','?
    ;

enum_member
    : identifier ('=' expr)
    ;

interface_members
    // Expected to be in this order, we will warning otherwise
    : (attr* constructor_definition)*
    | (attr* decl_func_member)*
    | (attr* decl_operator_member)*
    | macro_inline_call*
    ;

extend_members
    // Expected to be in this order, we will warning otherwise
    : stmt_import*
    | decl_const*
    | constructor_decl_with_visibility*
    | func_member_decl_with_visibility*
    | operator_member_decl_with_visibility*
    | macro_inline_call*
    ;    

decl_field
    : pre_decl ('let' | 'var') identifier ':' type EOS
    ;

decl_unit
    : pre_decl 'unit' identifier ('=' unit_expr)? EOS
    ;

unit_expr
    : identifier ('^' literal_integer)?
    | literal_float
    | literal_integer
    | unit_expr ('/' '*') unit_expr
    ;

attr_arguments
    : '(' attr_argument_list? ')'
    ;

attr_argument_list
    : attr_argument (',' attr_argument)*
    ;

attr_argument
    : literal
    | '.' identifier
    ;

// ------------------------------------------------------------------
// Functions
// ------------------------------------------------------------------

func_member_decl_with_visibility
    : pre_decl decl_func_member
    ;

decl_global_func
    : pre_decl func_static_regular_decl_part
    | pre_decl func_static_property_decl_part
    ;        

decl_func_member
    : func_this_regular_decl_part
    | func_this_property_decl_part
    | func_this_array_decl_part
    | func_static_regular_decl_part
    | func_static_property_decl_part
    ;

func_this_regular_decl_part
    : func_pre_modifier* 'func' func_this func_regular_part
    ;

func_this_property_decl_part
    : func_pre_modifier* 'func' func_this func_property_part
    ;

func_this_array_decl_part
    : func_pre_modifier* 'func' func_this '[' identifier ':' type ']' type_func_return property_constraint* property_body
    ;

func_static_regular_decl_part
    : func_pre_modifier* 'func' func_regular_part
    ;

func_static_property_decl_part
    : func_pre_modifier* 'func' func_property_part
    ;    

func_regular_part
    : identifier_with_generic_parameters parameters type_func_return? throws_constraint? func_constraint* func_body
    ;

func_property_part
    : identifier_with_generic_parameters type_func_return property_constraint* property_body
    ;

func_pre_modifier
    : 'partial'
    | 'unsafe'
    | async
    | 'mutable'
    ;

func_this
    :  ('this' | 'mutable' 'this')
    ;

func_constraint
    : where_constraint
    | requires_constraint
    ;

throws_constraint
    : 'throws' type_qualified (',' type_qualified)*
    ;

property_constraint
    : requires_constraint
    ;

requires_constraint
    : 'requires' expr
    ;

func_global_modifier
    : 'partial'? 'unsafe'?
    ;

constructor_decl_with_visibility
    : constructor_definition_with_visibility func_body
    ;

constructor_definition_with_visibility
    : pre_decl constructor_definition
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

type_func_return
    : '->' attr* type
    ;

func_body
    : func_expr_body EOS
    | expr_block
    ;

property_body
    : func_expr_body EOS
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

func_expr_body
    : '=>' expr
    ;

func_block_body
    : expr_block
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
// Operators
// ------------------------------------------------------------------

operator_member_decl_with_visibility
    : pre_decl decl_operator_member
    ;

decl_operator_member
    : operator_part
    ;

operator_pre_modifier
    : 'partial'
    | 'unsafe'
    ;

operator_binary
    : token1='+' 
    | token1='-' 
    | token1='*' 
    | token1='/' 
    | token1='%' 
    | token1='&'
    | token1='|' 
    | token1='^' 
    | token1='<' token2='<' 
    | token1='>' token2='>' 
    | token1='==' 
    | token1='<' 
    | token1='>' 
    | token1='<=' 
    | token1='>='
    ;

operator_unary
    : '+' | '-' | '~'
    ;

operator_part
    : operator_pre_modifier* 'operator' 'unary'  operator_unary parameters type_func_return func_body
    | operator_pre_modifier* 'operator' 'binary' operator_binary parameters type_func_return func_body
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

layout_parameters
    : '[' identifier ']'    // Generic parametrized SOA
    | '[' literal_integer? '|' ']'  // SOA
    | '[' ']'               // AOS
    ;

generic_parameters
    : '`' '<' generic_parameter (',' generic_parameter)* '>'  // full form of generic parameters: Dictionary`<TKey, TValue>
    | generic_parameter_simple+ // simple form of generic parameters e.g Dictionary`TKey`Tvalue
    ;

generic_arguments
    : '`' '<' generic_argument (',' generic_argument)* '>' // full form: Dictionary`<int, string>
    | generic_argument_simple+ // simple form: Dictionary`int`string 
    ;

generic_parameter
    : identifier ('=' generic_argument)?
    | lifetime
    ;

generic_parameter_simple
    : '`' identifier
    | lifetime
    ;

generic_argument
    : type
    | literal
    | lifetime
    ;
generic_argument_simple
    : '`' (identifier | literal_integer | literal_bool | type_primitive)
    | lifetime
    ;

lifetime
    : '#' identifier
    ;

qualified_func
    : qualified_name
    ;

qualified_name
    : module_path* identifier_with_generic_arguments 
    ;

qualified_name_no_generic_arguments
    : module_path* identifier
    ;

identifier_list
    : identifier (',' identifier)*
    ;

identifier
    : IDENTIFIER   // also might add macro_inline_call
    | 'alias'
    | 'are'
    | 'attr'
    | 'binary'
    | 'can'
    | 'case'
    | 'constructor'
    | 'enum'
    | 'exclusive'
    | 'extern'
    | 'inherits'
    | 'extend'
    | 'get'
    | 'has'
    | 'immutable'
    | 'implements'
    | 'import'
    | 'in'
    | 'indirect'
    | 'interface'
    | 'isolated'
    | 'kind'
    | 'layout'    
    | 'lifetime'
    | 'module'
    | 'macro'
    | 'mutable'
    | 'operator'
    | 'ownership'
    | 'permission'
    | 'partial'
    | 'pub'
    | 'readable'
    | 'requires'
    | 'rooted'
    | 'rw'
    | 'set'
    | 'shared'
    | 'static'
    | 'struct'
    | 'throws'
    | 'transient'
    | 'type'
    | 'unary'
    | 'union'
    | 'unique'
    | 'unit'
    | 'where'
    // Macro tokens
    | 'identifier'
    | 'expr'
    | 'stmt'
    | 'literal'
    | 'token'
    ;

module_path
    : identifier '::'
    ;

parameters
    : '(' parameter_list? ')'
    ;

parameter_list
    : parameter (',' parameter)*
    ;

parameter
    : parameter_name=identifier ':' attr* type
    ;

visibility
    : 'pub' 
    ;

// ------------------------------------------------------------------
// Statements
// ------------------------------------------------------------------

stmt
    : attr* 
        ( stmt_let_var
        | stmt_for
        | stmt_while
        | stmt_continue
        | stmt_break
        | stmt_return
        | stmt_expr
        )
    ;

stmt_let_var
    : ('let' | 'var') identifier ':' type ('=' expr) EOS
    ;

stmt_for
    : (async | await)? 'for' identifier 'in' expr expr_block
    ;

stmt_while
    : 'while' expr expr_block
    ;

stmt_return
    : 'return' expr? EOS
    ;

stmt_continue
    : 'continue' EOS
    ;

stmt_break
    : 'break' EOS
    ;

stmt_expr
    : expr_stmt EOS
    ;

// ------------------------------------------------------------------
// Expressions
// ------------------------------------------------------------------

expr_stmt
    : expr
    | expr_assignable ('=' | '+=' | '-=' | '*=' | '/=' | '&=' | '|=' | '^=' | '<' '<' '=' |  '>' '>' '=' | '%=' ) expr EOS
    ;

expr
    : 'unsafe'? (expr_with_block | expr_without_block)
    | expr bop='|>' expr
    ;

expr_with_block
    : expr_block
    | expr_if_with_block
    ;

// If an if expr starts to use expr_block, it should use them all along
expr_if_with_block
    : 'if' expr_unary_or_binary 'then' expr_block ('else' expr_with_block)?
    ;

expr_without_block
    : expr_unary_or_binary
    | expr_ref
    | expr_out
    | expr_address_of
    | expr_if
    | expr_match
    | expr_range
    | expr_anonymous_func
    // struct constructor
    | module_path* (identifier | method_call) list_initializer
    // Here the permission is mandatory otherwise the expr_unary_or_binary will a default constructor (with default permission)
    | type_permission_explicit expr_identifier_or_method_call_with_optional_generics
    // Here the permission is optional
    | type_permission_explicit? expr_constructor_array
    ;

expr_out
    // out only supports a path (because that's the parameter declaration that says what is the lifetime/ownership/permission)
    : 'out' expr_unary_or_binary
    // return a ref
    | 'out' expr_ref
    ;

expr_ref
    : 'ref' lifetime? type_ownership_explicit? type_permission_explicit? expr_unary_or_binary
    ;

expr_address_of
    : '&' expr_unary_or_binary
    ;

expr_range
    : expr_unary_or_binary? '.' '.' expr_unary_or_binary?
    ;

// If an if expr starts to use expr_without_block, it should use them all along
expr_if
    : 'if' expr_unary_or_binary 'then' expr_without_block ('else' expr_without_block)+
    ;

expr_match
    : 'match' expr_unary_or_binary '{' expr_case (',' expr_case)* ','? '}'
    ;

expr_case
    : 'case' expr_pattern func_expr_body
    ;

expr_pattern
    // case _ => ...
    : expr_discard
    // case int x => ...
    | type identifier?
    // case 0, 1, 2 => ...
    | literal (',' literal)*
    // case .ENUM_VALUE1, .ENUM_VALUE2 => ...
    | '.' identifier (',' '.' identifier)*
    // TODO add more patterns (range, ...etc.)
    ;

expr_block
    : '{' stmt* '}'
    ;


expr_unary_or_binary
    : expr_unary
    | expr_unary_or_binary type_measure // implicitly convert to *, so same precedence
    | expr_unary_or_binary bop=('*'|'/'|'%') expr_unary_or_binary
    | expr_unary_or_binary ('<' '<' | '>' '>') expr_unary_or_binary // we are departing from the classical C precedence here
    | expr_unary_or_binary bop=('+'|'-') expr_unary_or_binary
    | expr_unary_or_binary bop='as' type
    | expr_unary_or_binary bop='is' (identifier ':')? type 
    | expr_unary_or_binary bop='is' 'not' type 
    | expr_unary_or_binary bop=('<=' | '>=' | '<' | '>') expr_unary_or_binary
    | expr_unary_or_binary bop=('==' | '<' '>') expr_unary_or_binary
    | expr_unary_or_binary bop='&' expr_unary_or_binary
    | expr_unary_or_binary bop='^' expr_unary_or_binary
    | expr_unary_or_binary bop='|' expr_unary_or_binary
    | expr_unary_or_binary bop='&&' expr_unary_or_binary
    | expr_unary_or_binary bop='||' expr_unary_or_binary
    ;

expr_unary
    : expr_primary1 expr_member_path*
    // Note that if ownership / permission are present, the only expr_primary2 supported behind
    // is method call
    | expr_primary2 expr_member_path*
    | '.'  // the dot expr is a place holder for the piped value
    | expr_new
    | 'throw' expr_unary_or_binary
    | 'catch'? 'try' expr_unary_or_binary
    | async expr_unary_or_binary
    | await expr_unary_or_binary    
    | prefix=('+'|'-') expr_unary_or_binary
    | prefix=('~'|'not') expr_unary_or_binary
    ;

expr_anonymous_func
    : (async | await)? 'func' generic_arguments? arguments? type_func_return? func_expr_body
    | (async | await)? identifier func_expr_body
    | (async | await)? '(' identifier (',' identifier)* ')' func_expr_body
    ;

expr_assignable
    : expr_primary1 expr_member_path+ // assignable exprs that requires at least one member path
    | expr_primary2 expr_member_path* // assignable exprs that can be assigned directly
    ;

// These exprs cannot be assigned directly
expr_primary1
    : 'this'                    #expr_this
    | literal                   #expr_literal
    // An enum or union constructor
    | '.' (identifier | method_call) #expr_enum_or_union
    ;

// These exprs can be assigned directly
expr_primary2
    // A local variable: e.g x
    // A local func call: e.g f(x)
    // A module variable: e.g core::sub::x
    // A module func call: e.g core::sub::f(x)
    // A constructor by value (MyStruct): e.g var x = MyStruct
    // A constructor by value (MyStruct): e.g var x = MyStruct(1,2,3)
    : expr_identifier_or_method_call_with_optional_generics
    // Group and Tuple/List expr
    | '(' expr (',' expr)* ')'
    // Dereferencing
    | '*' expr_unary_or_binary
    // Discard '_'
    | expr_discard
    ;

expr_identifier_or_method_call_with_optional_generics
    // A local variable: e.g x
    // A local func call: e.g f(x)
    // A module variable: e.g core::sub::x
    // A module func call: e.g core::sub::f(x)
    // A constructor by value (MyStruct): e.g var x = MyStruct
    // A constructor by value (MyStruct): e.g var x = MyStruct(1,2,3)
    : module_path* (identifier | method_call)
    // A field of a generic type, e.g:
    // - MyStruct`<int>.x / MyStruct`<int>.f(x)
    // - core::sub::MyStruct`<int>.x / core::sub::MyStruct`<int>.f(x)
    | module_path* identifier generic_arguments expr_dot_path
    ;

expr_member_path
    : expr_dot_path
    | expr_indexer_path
    ;

expr_dot_path
    : '.' identifier
    | '.' method_call
    ;

expr_indexer_path
    : '[' expr ']'
    ;

// A const expr path is an identifier, or a path to a const
// that could involve a generic instance type
const_path
    : qualified_name_no_generic_arguments // path for a const declared in a module: e.g core::module::const_field
    | qualified_name '.' identifier // generic instance const field: core::module::MyType<true>.const_field
    ;

expr_new
    : 'new' lifetime? type_ownership_explicit? type_permission_explicit? expr_new_constructor
    ;

expr_new_constructor
    : expr_constructor_struct
    | expr_constructor_array
    | expr_constructor_string
    ; 

expr_constructor_struct
    : module_path* identifier_with_generic_arguments arguments? list_initializer?
    ;

expr_constructor_array
    : '[' type ']' list_initializer?                    // with list initializer
    | '[' type ']' '(' expr ')' list_initializer? // with list initializer
    | '[' type ']' '(' expr ',' expr ')'    // with func initializer
    | '[' 'u8' ']' literal_string                       // with list initializer
    ;

expr_constructor_string
    : literal_string
    ;

expr_discard
    : '_'
    ;

list_initializer
    : '{' expr (',' expr)* '}'
    ;  

method_call
    : identifier_with_generic_arguments arguments
    ;

expr_list
    : expr (',' expr)*
    ;

arguments
    : '(' expr_list? ')'
    ;

// ------------------------------------------------------------------
// Macros
// ------------------------------------------------------------------

decl_macro
    : visibility? ('partial' | 'extern')? 'macro' identifier '{' macro_members '}'
    ;

macro_members
    : attr
    | decl_macro_func
    ;

decl_macro_func
    : 'func' '(' macro_func_parameter* ')' macro_func_body
    | 'func' '[' macro_func_parameter* ']' macro_func_body
    | 'func' '{' macro_func_parameter* '}' macro_func_body
    ;

macro_func_parameter
    : macro_identifier ':' macro_func_parameter_expr*
    ;

macro_func_parameter_expr
    : 'identifier'
    | 'lifetime'
    | 'expr'
    | 'stmt'
    | 'literal'
    | 'token'
    | macro_literal_token
    | '?' macro_func_parameter_expr
    | '*' macro_func_parameter_expr
    | '+' macro_func_parameter_expr
    | '(' macro_func_parameter_expr+ ')'
    ;

macro_func_body
    : '=>' macro_stmt EOS
    // here the number of leading $ must match the trailing $
    // but we can't express here this without custom hooks
    // so we declare only 3 levels
    | '{' '$' macro_stmt* '$' '}'
    | '{' '$' '$' macro_stmt* '$' '$' '}'
    | '{' '$' '$' '$' macro_stmt* '$' '$' '$' '}'
    ;

macro_stmt
    : macro_tokens
    | '(' macro_stmt* ')'
    | '[' macro_stmt* ']'
    | '{' macro_stmt* '}'
    // here the number of leading $ must match the trailing $
    // but we can't express here this without custom hooks
    // so we declare only 3 levels
    | '<' '$' macro_command '$' '>'
    | '<' '$' '$' macro_command '$' '$' '>'
    | '<' '$' '$' '$' macro_command '$' '$' '$' '>'
    ;

macro_command
    : macro_expr
    | 'for' macro_identifier 'in' macro_identifier '{'?
    | 'if' macro_expr 'then' '{'?
    | '}'? 'else' '{'?
    | '{'
    | '}'
    ;

macro_tokens
    // All tokens except group tokens '('|')'|'['|']'|'{'|'}'
    // as they are parsed below to match balanced groups
    :'_'|'-'|'-='|'->'|','|':'|'!'|'.'|'@'|'*'|'*='|'/'|'/='|'&'|'&&'|'&='|'#'|'%'|'%='|'`'|'^'|'^='|'+'|'+='|'<'|'<='|'='|'=='|'=>'|'>'|'>='|'|'|'|='|'||'|'|>'|'~'|'alias'|'are'|'as'|'async'|'attr'|'await'|'binary'|'bool'|'break'|'can'|'case'|'catch'|'const'|'constructor'|'continue'|'else'|'enum'|'exclusive'|'expr'|'inherits'|'extend'|'extern'|'f32'|'f64'|'for'|'func'|'get'|'has'|'i16'|'i32'|'i64'|'i8'|'identifier'|'if'|'immutable'|'implements'|'import'|'in'|'indirect'|'int'|'interface'|'is'|'isolated'|'kind'|'layout'|'let'|'lifetime'|'literal'|'macro'|'rw'|'match'|'module'|'mutable'|'new'|'not'|'operator'|'out'|'ownership'|'permission'|'partial'|'pub'|'readable'|'ref'|'requires'|'return'|'rooted'|'set'|'shared'|'stmt'|'static'|'struct'|'then'|'this'|'throw'|'throws'|'token'|'transient'|'try'|'type'|'u16'|'u32'|'u64'|'u8'|'uint'|'unary'|'union'|'unique'|'unit'|'unsafe'|'v128'|'v256'|'var'|'where'|'while'
    | literal
    | IDENTIFIER
    | lifetime
    | macro_identifier
    | macro_literal_token
    | '(' macro_tokens* ')'
    | '[' macro_tokens* ']'
    | '{' macro_tokens* '}'
    ;

macro_expr
    : macro_identifier
    | literal
    | '(' macro_expr ')'
    | macro_expr bop=('<=' | '>=' | '<' | '>') macro_expr
    | macro_expr bop=('==' | '<' '>') macro_expr
    | macro_expr bop='&&' macro_expr
    | macro_expr bop='||' macro_expr    
    ;

macro_identifier
    : '$' identifier
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
    : type_non_const
    | type_const
    ;

type_non_const
    : type_permission_explicit? type_core
    | type_ref
    | type_pointer
    ;

// Core types are types without a qualifier (ref, const, mutable, pointer)
// Usable by e.g decl_type
type_core
    : type_primitive 
    | type_qualified type_struct_layout?
    | type_array
    | type_fixed_array
    | type_tuple
    | type_measure
    | type_union
    | type_func // TODO: should it be a type_core or not?
    ;

// For SOA and AOS
type_struct_layout
    : '[' literal_integer? '|'  ']' // SOA
    | '[' ']'                       // AOS
    ;

type_primitive
    : type_bool
    | type_integer
    | type_float
    | type_vector
    ;

type_bool
    : 'bool'
    ;

type_integer
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

type_float
    : 'f32'
    | 'f64'
    ;

type_vector
    : 'v128'
    | 'v256'
    ;    

type_qualified
    : qualified_name
    ;

type_const
    : 'const' type_non_const // avoid const const
    ;

type_ref
    : ref_or_out lifetime? type_ownership_explicit? type
    //              lifetime     ownership                                 permission
    | ref_or_out '`' '<' lifetime ',' type_owrnership_identifier_or_generic ',' type_permission_identifier_or_generic ',' type '>'
    ;

ref_or_out
    : 'ref'
    | 'out'
    ;

type_tuple
    : '(' type (',' type)+ ')'
    ;

type_owrnership_identifier_or_generic
    : type_owrnership_identifier
    | identifier
    ;

type_ownership_explicit
    : '`' type_owrnership_identifier
    ;

type_owrnership_identifier
    : 'unique'
    | 'shared'
    | 'rooted'
    | 'transient'
    ;

type_permission_explicit
    : '`' type_permission_identifier
    ;

type_permission_identifier_or_generic
    : type_permission_identifier
    | identifier
    ;

type_permission_identifier
    : 'mutable'
    | 'immutable'
    | 'readable'
    | 'const'
    ;

type_union
    : '.' identifier
    ;

type_array
    : '[' type ']' 
    ;

// We will validate the <expr> for the dimension in the semantic analysis
type_fixed_array
    : '[' type ',' expr ']' 
    ;    

type_pointer
    : '*' (type_primitive | type_qualified)
    ;

unit
    : '\'' identifier
    ;

type_measure
    : (type_integer | type_float) unit
    ;

type_func
    : async? 'func' generic_parameters? parameters type_func_return? throws_constraint? func_constraint*
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
    : literal_without_qualifier literal_type_qualifier?
    ;

literal_without_qualifier
    : literal_integer
    | literal_float
    | literal_bool
    | literal_char
    | literal_string
    ;

literal_type_qualifier
    : '!' (type_integer | type_float)
    ;

// ------------------------------------------------------------------
// Keywords (so used in method body / exprs)
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
NEW: 'new';
NOT: 'not';
OUT: 'out';
REF: 'ref';
RETURN: 'return';
THEN: 'then';
THIS: 'this';
THROW: 'throw';
TRY: 'try';
UNSAFE: 'unsafe';
VAR: 'var';
WHILE: 'while';

// ------------------------------------------------------------------
// Non Keywords (can be used as identifiers)
// ------------------------------------------------------------------

ALIAS: 'alias';
ARE: 'are';
ATTR: 'attr';
BINARY: 'binary';
CAN: 'can';
CASE: 'case';
CONSTRUCTOR: 'constructor';
ENUM: 'enum';
EXCLUSIVE: 'exclusive';
EXTERN: 'extern';
INHERITS: 'inherits';
EXTEND: 'extend';
GET: 'get';
HAS: 'has';
IMMUTABLE: 'immutable';
IMPLEMENTS: 'implements';
IMPORT: 'import';
IN: 'in';
INDIRECT: 'indirect';
INTERFACE: 'interface';
ISOLATED: 'isolated';
KIND: 'kind';
LAYOUT: 'layout';
LIFETIME: 'lifetime';
MODULE: 'module';
MACRO: 'macro';
MUTABLE: 'mutable';
OPERATOR: 'operator';
OWNERSHIP: 'ownership';
PERMISSION: 'permission';
PARTIAL: 'partial';
PUB: 'pub';
READABLE: 'readable';
REQUIRES: 'requires';
ROOTED: 'rooted';
RW: 'rw';
SET: 'set';
SHARED: 'shared';
STATIC: 'static';
STRUCT: 'struct';
TRANSIENT: 'transient';
THROWS: 'throws';
TYPE: 'type';
UNARY: 'unary';
UNION: 'union';
UNIQUE: 'unique';
UNIT: 'unit';
WHERE: 'where';

// Begin MACRO Tokens
MACRO_TOKEN_IDENTIFIER: 'identifier';
MACRO_TOKEN_EXPRESSION: 'expr';
MACRO_TOKEN_STATEMENT: 'stmt';
MACRO_TOKEN_LITERAL: 'literal';
MACRO_TOKEN_TOKEN: 'token';
// End MACRO Tokens

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

UNDERSCORE: '_'+;
IDENTIFIER: Identifier;

// This is not fully correct, that should not be hidden, 
// but we don't want to handle correctly NEW_LINE in this grammar
EOS: [\n;] -> channel(HIDDEN);

// 1 byte symbols
EXCLAMATION: '!';
// DOUBLE_QUOTE: '"';
HASH: '#';
DOLLAR: '$';
PERCENT: '%';
AMPERSAND: '&';
SINGLE_QUOTE: '\'';
LEFT_PAREN: '(';
RIGHT_PAREN: ')';
STAR: '*';
PLUS: '+';
COMMA: ',';
MINUS: '-';
DOT: '.';
SLASH: '/';
COLON: ':';
SEMI_COLON: ';';
LESS_THAN: '<';
EQUAL: '=';
GREATER_THAN: '>';
QUESTION: '?';
AT: '@';
LEFT_BRACKET: '[';
BACKSLASH: '\\';
RIGHT_BRACKET: ']';
CIRCUMFLEX: '^';
// UNDERSCORE: '_'+;
BACKTICK: '`';
LEFT_BRACE: '{';
VerticalBar: '|';
RIGHT_BRACE: '}';
TILDE: '~';

// 2-3 byte symbols
DOUBLE_AMPERSAND: '&&';
DOUBLE_VERTICAL_BAR: '||';
DOUBLE_EQUAL: '==';
SLASH_EQUAL: '/=';
PERCENT_EQUAL: '%=';
AMPERSAND_EQUAL: '&=';
STAR_EQUAL: '*=';
PLUS_EQUAL: '+=';
MINUS_EQUAL: '-=';
TILDE_EQUAL: '~=';
CIRCUMFLEX_EQUAL: '^=';
VERTICAL_BAR_EQUAL: '|=';
LESS_THAN_OR_EQUAL: '<=';
GREATER_THAN_OR_EQUAL: '>=';
EQUAL_GREATER_THAN: '=>';
MINUS_GREATER_THAN: '->';
VERTICAL_BAR_GREATER_THAN: '|>';

DOUBLE_COLON: '::';

DOUBLE_DOT: '..';
DOUBLE_DOT_LESS_THAN: '..<';

TRIPLE_EQUAL: '===';


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
