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
    : decl_module_member* 
    ;

decl_module_member
    : decl_module
    | decl_import
    | decl_struct
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
    | decl_empty
    // | macro_inline_call
    ;    

// ------------------------------------------------------------------
// Top level Declaration
// ------------------------------------------------------------------    

decl_empty
    : eos
    ;

decl_module
    : pre_decl 'partial'? 'module' module_path* identifier eos
    ;

decl_import
    : 'import' module_path* import_identifier eos
    ;

import_identifier
    : identifier                #import_identifier_single
    | '*'                       #import_identifier_wildcard
    | '{' identifier_list '}'   #import_identifier_list
    ;

decl_attr
    : pre_decl 'attr' identifier parameters eos
    ;

attr
    : '@' identifier attr_arguments?
    ;

decl_const
    : pre_decl 'const' identifier (':' type)? '=' expr eos
    ;

decl_static
    : 'static' identifier (':' type)? '=' expr eos
    ;

decl_struct
    : pre_decl 'partial'? 'rw'? 'struct' identifier /*layout_parameters?*/ generic_parameters? parameters? implement_contraint* where_constraint* struct_body
    ;

decl_interface
    : pre_decl 'partial'? 'interface' identifier_with_generic_parameters parameters? inherits_constraint* where_constraint* interface_body
    ;

decl_extend
    : pre_decl 'extend' generic_parameters? FOR type_qualified implement_contraint* where_constraint* extend_body
    ;

decl_union
    : pre_decl 'union' identifier_with_generic_parameters where_constraint* union_body
    ;

decl_enum
    : pre_decl 'enum' identifier (':' type_primitive)? enum_body
    ;

decl_type
    : pre_decl 'type' identifier_with_generic_parameters where_constraint* '=' type_core eos
    ;

decl_alias_type
    : pre_decl 'alias' 'type' identifier '=' type_core eos
    ;

decl_alias_func
    : pre_decl 'alias' 'func' identifier '=' qualified_name (DOT identifier)? eos
    ;

pre_decl
    : attr* visibility?
    ;

where_constraint
    : 'where' identifier (',' identifier)* ':' where_constraint_part (',' where_constraint_part)*
    // | 'where' lifetime (',' lifetime)* ':' where_constraint_lifetime_part (',' where_constraint_lifetime_part)*
    ;

where_constraint_part
    : 'is' type     #where_constraint_part_is
    | 'kind' ('enum' | 'union' | 'rw'? 'struct' | 'interface' | 'unit' | 'layout' | 'ownership' | 'permission') #where_constraint_part_kind // TODO: should we add e.g | 'integer' | 'float' | 'number'?
    | 'has' 'constructor' identifier? parameters #where_constraint_part_has
    ;

// where_constraint_lifetime_part
//     : 'has' 'lifetime' ('<' | '<=') lifetime
//     | 'can' 'new'
//     ;

implement_contraint
    : 'implements' type_qualified
    ;

inherits_constraint
    : 'inherits' type_qualified
    ;

body_block_open
    : '='
    | '==='
    ;

struct_body
    : eos                                  #struct_body_no_members
    | body_block_open eos struct_member*   #struct_body_with_members
    ;    

interface_body
    : eos                                   #interface_body_no_members
    | body_block_open eos interface_member* #interface_body_with_members
    ;

extend_body
    : eos                                   #extend_body_no_members
    | body_block_open eos extend_member*    #extend_body_with_members
    ; 

union_body
    : body_block_open eos? union_members
    ;

enum_body
    : body_block_open eos? enum_members
    ;

// members are indented
struct_member
    // Expected to be in this order, we will warning otherwise
    : decl_import
    | decl_const
    | decl_field
    | constructor_decl_with_visibility
    | func_member_decl_with_visibility
    | operator_member_decl_with_visibility
//    | macro_inline_call
    ;    

union_members
    : union_member (',' eos? union_member)* ','?
    ; 

union_member
    : '.' identifier parameters?  #union_member_regular
    | type                        #union_member_type
    ;

enum_members
    : enum_member (',' enum_member)* ','?
    ;

enum_member
    : identifier ('=' expr)
    ;

interface_member
    : attr* interface_member_without_attr
    ;

interface_member_without_attr
    // Expected to be in this order, we will warning otherwise
    : constructor_definition
    | decl_func_member
    | decl_operator_member
//    | macro_inline_call
    ;

extend_member
    // Expected to be in this order, we will warning otherwise
    : decl_import
    | decl_const
    | constructor_decl_with_visibility
    | func_member_decl_with_visibility
    | operator_member_decl_with_visibility
//    | macro_inline_call*
    ;    

decl_field
    : pre_decl ('let' | 'var') identifier ':' type eos
    ;

decl_unit
    : pre_decl 'unit' identifier ('=' unit_expr)? eos
    ;

unit_expr
    : identifier ('^' literal_integer)?   #unit_expr_identifer_with_exp
    | literal_float                       #unit_expr_literal
    | literal_integer                     #unit_expr_literal
    | unit_expr ('/' '*') unit_expr       #unit_expr_mul_or_div
    ;

attr_arguments
    : '(' attr_argument_list? ')'
    ;

attr_argument_list
    : attr_argument (',' attr_argument)*
    ;

attr_argument
    : literal          #attr_argument_literal
    | '.' identifier   #attr_argument_identifier
    ;

// ------------------------------------------------------------------
// Functions
// ------------------------------------------------------------------

func_member_decl_with_visibility
    : pre_decl decl_func_member
    ;

decl_global_func
    : pre_decl func_static_regular_decl_part    #decl_global_func_regular
    | pre_decl func_static_property_decl_part   #decl_global_func_property
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
    : 'partial'  #func_pre_modifier_regular
    | 'unsafe'   #func_pre_modifier_regular
    | async      #func_pre_modifier_async
    | 'rw'  #func_pre_modifier_regular
    ;

func_this
    : 'this'        #func_this1
    | 'rw' 'this'   #func_this2
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
    : func_expr_body eos #func_body_direct
    | expr_block         #func_body_with_block
    ;

property_body
    : func_expr_body eos  #property_body_direct
    | property_block_body #property_body_with_block
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
    : 'async'                                             #async_direct
    | 'async' '`' async_await_generic_short_param         #async_parameterized
    | 'async' '`' '<' async_await_generic_long_param '>'  #async_parameterized_long
    ;

await
    : 'await'                                             #await_direct
    | 'await' '`' async_await_generic_short_param         #await_parameterized
    | 'await' '`' '<' async_await_generic_long_param '>'  #await_parameterized_long
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
    : token1='+'  #operator_binary_1  
    | token1='-'  #operator_binary_1 
    | token1='*'  #operator_binary_1 
    | token1='/'  #operator_binary_1 
    | token1='%'  #operator_binary_1 
    | token1='&'  #operator_binary_1
    | token1='|'  #operator_binary_1 
    | token1='^'  #operator_binary_1 
    | token1='<' token2='<' #operator_binary_1
    | token1='>' token2='>' #operator_binary_1
    | token1='==' #operator_binary_1 
    | token1='<'  #operator_binary_1
    | token1='>'  #operator_binary_1
    | token1='<=' #operator_binary_1 
    | token1='>=' #operator_binary_1
    ;

operator_unary
    : '+' | '-' | '~'
    ;

operator_part
    : operator_pre_modifier* 'operator' 'unary'  operator_unary parameters type_func_return func_body    #operator_part_unary
    | operator_pre_modifier* 'operator' 'binary' operator_binary parameters type_func_return func_body   #operator_part_binary
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

// layout_parameters
//     : '[' identifier ']'  #layout_  // Generic parametrized SOA
//     | '[' literal_integer? '|' ']'  // SOA
//     | '[' ']'               // AOS
//     ;

generic_parameters
    : '`' '<' generic_parameter (',' generic_parameter)* '>'  #generic_parameters_long // full form of generic parameters: Dictionary`<TKey, TValue>
    | generic_parameter_simple+                               #generic_parameters_simple // simple form of generic parameters e.g Dictionary`TKey`Tvalue
    ;

generic_arguments
    : '`' '<' generic_argument (',' generic_argument)* '>'    #generic_arguments_long // full form: Dictionary`<int, string>
    | generic_argument_simple+                                #generic_arguments_simple // simple form: Dictionary`int`string 
    ;

generic_parameter
    : identifier ('=' generic_argument)? #generic_parameter_identifier_and_argument
    | lifetime                           #generic_parameter_lifetime
    ;

generic_parameter_simple
    : '`' identifier #generic_parameter_identifier_simple
    | lifetime       #generic_parameter_lifetime_simple
    ;

generic_argument
    : type
    | literal
    | lifetime
    ;
generic_argument_simple
    : '`' (identifier | literal_integer | literal_bool | type_primitive) #generic_argument_value_simple
    | lifetime                                                           #generic_argument_lifetime_simple
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
    : attr* stmt_simple
    ;

stmt_simple
    : stmt_let_var
    | stmt_for
    | stmt_while
    | stmt_continue
    | stmt_break
    | stmt_return
    | stmt_expr
    ;

stmt_let_var
    : let_or_var identifier ':' type ('=' expr)? eos  #stmt_let_var_with_explicit_type
    | let_or_var identifier '=' expr eos              #stmt_let_var_implicit_type
    ;
let_or_var
    : 'let'
    | 'var'
    ;

stmt_for
    : async_or_await? 'for' identifier 'in' expr expr_block
    ;

async_or_await
    : async
    | await
    ;    

stmt_while
    : 'while' expr expr_block
    ;

stmt_return
    : 'return' expr? eos
    ;

stmt_continue
    : 'continue' eos
    ;

stmt_break
    : 'break' eos
    ;

stmt_unsafe
    : 'unsafe' expr_block
    ;

stmt_expr
    : 'unsafe'? expr eos
    ;

stmt_assignment
    : expr operator_assignment 'unsafe'? expr eos
    ;

operator_assignment
    : '=' | '+=' | '~=' | '-=' | '*=' | '/=' | '&=' | '|=' | '^=' | '<<=' |  '>>=' | '%='
    ;

// ------------------------------------------------------------------
// Expressions
// ------------------------------------------------------------------

expr
    : expr_primary              #expr_unary_primary
    | type_permission_explicit expr #expr_with_permission
    | 'new' lifetime? type_ownership_explicit? type_permission_explicit? expr_new_constructor #expr_new
    | expr_block                #expr_with_block1
    | 'throw' expr              #expr_unary_throw
    | 'catch'? 'try' expr       #expr_unary_catch_try
    | operator_async_await expr #expr_unary_async_await
    | 'if' expr 'then' expr ('else' expr)*  #expr_if
    | 'match' expr match_block  #expr_match
    | expr_anonymous_func       #expr_func
    | expr arguments            #expr_func_call
    | expr list_initializer     #expr_struct_constructor
    | expr '.' method_call      #expr_member_method_call
    | expr '.' identifier       #expr_member_field_or_property
    | expr '[' expr ']'         #expr_member_array_indexer
    | '*' expr                  #expr_unary_deref
    | '&' lifetime? type_ownership_explicit? type_permission_explicit? expr #expr_unary_ref
    | prefix=('+'|'-') expr     #expr_unary_plus_or_minus
    | prefix=('~'|'not') expr   #expr_unary_not    
    | expr bop=operator_range expr? #expr_binary_range
    | bop=operator_range expr       #expr_binary_range
    | bop=operator_range            #expr_binary_range
    | expr type_measure  #expr_binary_measure // implicitly convert to *, so same precedence
    | expr bop=('*'|'/'|'%') expr   #expr_binary
    | expr (bop='<' bop2='<' | bop='>' bop2='>') expr #expr_binary // we are departing from the classical C precedence here
    | expr bop=('+'|'-') expr #expr_binary
    | expr bop='as' type #expr_binary_as
    | expr bop='is' 'not'? type #expr_binary_is
    | expr bop=('<=' | '>=' | '<' | '>') expr #expr_binary
    | expr bop=('==' | '<>') expr #expr_binary
    | expr bop='&' expr #expr_binary
    | expr bop='^' expr #expr_binary
    | expr bop='|' expr #expr_binary
    | expr bop='&&' expr #expr_binary
    | expr bop='||' expr #expr_binary
    | expr bop='|>' expr #expr_binary_pipe
    ;

expr_primary
    // These exprs cannot be assigned directly
    : 'this'                    #expr_primary_this
    | literal                   #expr_primary_literal
    | '.'                       #expr_primary_dot // the dot expr is a place holder for the piped value
    | '.' identifier  #expr_primary_enum_or_union  // An enum or union constructor
    | '.' method_call #expr_primary_enum_or_union  // An enum or union constructor
    // These exprs can be assigned directly
    // A local variable: e.g x
    // A local func call: e.g f(x)
    // A module variable: e.g core::sub::x
    // A module func call: e.g core::sub::f(x)
    // A constructor by value (MyStruct): e.g var x = MyStruct
    // A constructor by value (MyStruct): e.g var x = MyStruct(1,2,3)
    | qualified_name  #expr_primary_qualified_name
    // Group and Tuple/List expr
    | '(' expr (',' expr)* ')'  #expr_primary_expr_list
    | expr_discard  #expr_primary_discard // Discard '_'
    | type_primitive #expr_primary_type_primitive
    ;

operator_range
    : '..'
    | '..<'
    ;

expr_block
    : '{' stmt* '}'
    ;    
   
expr_anonymous_func
    : operator_async_await? 'func' generic_arguments? arguments? type_func_return? func_expr_body #expr_anonymous_func_explicit
    | operator_async_await? identifier func_expr_body                                             #expr_anonymous_func_single_parameter
    | operator_async_await? '(' identifier (',' identifier)* ')' func_expr_body                   #expr_anonymous_func_multi_parameters
    ;

operator_async_await
    : async
    | await

    ;

match_block
    : '{' match_case_list? '}'
    ;

match_case_list
    : match_case (',' match_case)* ','?
    ;

match_case
    : 'case' match_case_pattern func_expr_body  #expr_case_regular
    | 'else' func_expr_body               #expr_case_else
    ;

match_case_pattern
    : type identifier?         #match_case_pattern_type // case int x => ...    
    | literal (',' literal)*   #match_case_pattern_literal // case 0, 1, 2 => ...    
    | '.' identifier (',' '.' identifier)* #match_case_pattern_enum // case .ENUM_VALUE1, .ENUM_VALUE2 => ...
    // TODO add more patterns (range, ...etc.)
    ;

// A const expr path is an identifier, or a path to a const
// that could involve a generic instance type
const_path
    : qualified_name_no_generic_arguments #const_path_simple // path for a const declared in a module: e.g core::module::const_field
    | qualified_name '.' identifier       #const_path_member // generic instance const field: core::module::MyType<true>.const_field
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
    : '[' type ']' list_initializer?                    #expr_constructor_array_with_list // with list initializer
    | '[' type ']' '(' expr ')' list_initializer?       #expr_constructor_array_with_ctor_and_list // with list initializer
    | '[' type ']' '(' expr ',' expr ')'                #expr_constructor_array_with_ctor_and_func // with func initializer
    | '[' 'u8' ']' literal_string                       #expr_constructor_array_literal_string // with list initializer
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

expr_argument_list
    : expr_argument (',' expr_argument)*
    ;

expr_argument
    : 'out'? expr
    ;

arguments
    : '(' expr_argument_list? ')'
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
    : 'func' '(' macro_func_parameter* ')' macro_func_body  #macro_func_with_parent
    | 'func' '[' macro_func_parameter* ']' macro_func_body  #macro_func_with_bracket
    | 'func' '{' macro_func_parameter* '}' macro_func_body  #macro_func_with_brace
    ;

macro_func_parameter
    : macro_identifier ':' macro_func_parameter_expr*
    ;

macro_func_parameter_expr
    : 'identifier'                     #macro_func_parameter_expr_kind
    | 'lifetime'                       #macro_func_parameter_expr_kind
    | 'expr'                           #macro_func_parameter_expr_kind
    | 'stmt'                           #macro_func_parameter_expr_kind
    | 'literal'                        #macro_func_parameter_expr_kind
    | 'token'                          #macro_func_parameter_expr_kind
    | macro_literal_token              #macro_func_parameter_expr_token
    | '?' macro_func_parameter_expr    #macro_func_parameter_expr_opt
    | '*' macro_func_parameter_expr    #macro_func_parameter_expr_star
    | '+' macro_func_parameter_expr    #macro_func_parameter_expr_plus
    | '(' macro_func_parameter_expr+ ')' #macro_func_parameter_expr_group
    ;

macro_func_body
    : '=>' macro_stmt eos             #macro_func_body_direct
    // here the number of leading $ must match the trailing $
    // but we can't express here this without custom hooks
    // so we declare only 3 levels
    | '{' '$' macro_stmt* '$' '}'                  #macro_func_body_nested
    | '{' '$' '$' macro_stmt* '$' '$' '}'          #macro_func_body_nested
    | '{' '$' '$' '$' macro_stmt* '$' '$' '$' '}'  #macro_func_body_nested
    ;

macro_stmt
    : macro_tokens         #macro_stmt_tokens
    | '(' macro_stmt* ')'  #macro_stmt_with_parent
    | '[' macro_stmt* ']'  #macro_stmt_with_bracket
    | '{' macro_stmt* '}'  #macro_stmt_with_brace
    // here the number of leading $ must match the trailing $
    // but we can't express here this without custom hooks
    // so we declare only 3 levels
    | '<' '$' macro_command '$' '>'                  #macro_stmt_command
    | '<' '$' '$' macro_command '$' '$' '>'          #macro_stmt_command
    | '<' '$' '$' '$' macro_command '$' '$' '$' '>'  #macro_stmt_command
    ;

macro_command
    : macro_expr                                         #macro_command_expr
    | 'for' macro_identifier 'in' macro_identifier '{'?  #macro_command_for
    | 'if' macro_expr 'then' '{'?                        #macro_command_if
    | '}'? 'else' '{'?                                   #macro_command_else
    | '{'                                                #macro_command_open_brace
    | '}'                                                #macro_command_close_brace
    ;

macro_tokens
    // All tokens except group tokens '('|')'|'['|']'|'{'|'}'
    // as they are parsed below to match balanced groups
    : macro_tokens_all                           #macro_tokens_direct
    | literal                                    #macro_tokens_literal
    | IDENTIFIER                                 #macro_tokens_identifier
    | lifetime                                   #macro_tokens_lifetime
    | macro_identifier                           #macro_tokens_macro_identifier
    | macro_literal_token                        #macro_tokens_macro_literal_token
    | '(' macro_tokens* ')'                      #macro_tokens_with_parent
    | '[' macro_tokens* ']'                      #macro_tokens_with_bracket
    | '{' macro_tokens* '}'                      #macro_tokens_with_braces
    ;

// TODO: this list should be updated automatically
macro_tokens_all
    : '_'|'-'|'-='|'->'|','|':'|'!'|'.'|'@'|'*'|'*='|'/'|'/='|'&'|'&&'|'&='|'#'|'%'|'%='|'`'|'^'|'^='|'+'|'+='|'<'|'<='|'='|'=='|'=>'|'>'|'>='|'|'|'|='|'||'|'|>'|'~'|'alias'|'are'|'as'|'async'|'attr'|'await'|'binary'|'bool'|'break'|'can'|'case'|'catch'|'const'|'constructor'|'continue'|'else'|'enum'|'exclusive'|'expr'|'inherits'|'extend'|'extern'|'f32'|'f64'|'for'|'func'|'get'|'has'|'i16'|'i32'|'i64'|'i8'|'identifier'|'if'|'immutable'|'implements'|'import'|'in'|'indirect'|'int'|'interface'|'is'|'isolated'|'kind'|'layout'|'let'|'lifetime'|'literal'|'macro'|'rw'|'match'|'module'|'new'|'not'|'operator'|'out'|'ownership'|'permission'|'partial'|'pub'|'readable'|'ref'|'requires'|'return'|'rooted'|'set'|'shared'|'stmt'|'static'|'struct'|'then'|'this'|'throw'|'throws'|'token'|'transient'|'try'|'type'|'u16'|'u32'|'u64'|'u8'|'uint'|'unary'|'union'|'unique'|'unit'|'unsafe'|'v128'|'v256'|'var'|'where'|'while'
    ;

macro_expr
    : macro_identifier                                      #macro_expr_identifier
    | literal                                               #macro_expr_literal
    | '(' macro_expr ')'                                    #macro_expr_nested
    | macro_expr bop=('<=' | '>=' | '<' | '>') macro_expr   #macro_expr_binary
    | macro_expr (bop='==' | bop='<' bop2='>') macro_expr   #macro_expr_binary
    | macro_expr bop='&&' macro_expr                        #macro_expr_binary
    | macro_expr bop='||' macro_expr                        #macro_expr_binary
    ;

macro_identifier
    : '$' identifier
    ;

macro_literal_token
    : MACRO_STRING_LITERAL
    ;

macro_inline_call
    : macro_identifier '(' macro_argument* ')' #macro_inline_call_with_parent
    | macro_identifier '[' macro_argument* ']' #macro_inline_call_with_bracket
    | macro_identifier '{' macro_argument* '}' #macro_inline_call_with_brace
    ;

macro_argument
    : macro_tokens            #macro_argument_tokens
    | '(' macro_argument* ')' #macro_argument_with_parent
    | '[' macro_argument* ']' #macro_argument_with_bracket
    | '{' macro_argument* '}' #macro_argument_with_braces
    ;

// ------------------------------------------------------------------
// Types
// ------------------------------------------------------------------

type
    : type_non_const
    | type_const
    ;

type_non_const
    : type_core_with_permission
    | type_ref
    | type_pointer
    ;

type_core_with_permission
    : type_permission_explicit? type_core
    ;

// Core types are types without a qualifier (ref, const, mutable, pointer)
// Usable by e.g decl_type
type_core
    : type_primitive 
//    | type_qualified type_struct_layout?
    | type_qualified
    | type_array
    | type_fixed_array
    | type_tuple
    | type_measure
    | type_union
    | type_func // TODO: should it be a type_core or not?
    ;

// // For SOA and AOS
// type_struct_layout
//     : '[' literal_integer? '|'  ']' // SOA
//     | '[' ']'                       // AOS
//     ;

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
    : ref_or_out lifetime? type_ownership_explicit? type #type_ref_simple
    //              lifetime     ownership                                 permission
    | ref_or_out '`' '<' lifetime ',' type_owrnership_identifier_or_generic ',' type_permission_identifier_or_generic ',' type '>' #type_ref_parameterized
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
    : 'rw'
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

eos
    : ';'
    | NEW_LINE
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
EXTEND: 'extend';
GET: 'get';
HAS: 'has';
IMMUTABLE: 'immutable';
IMPLEMENTS: 'implements';
IMPORT: 'import';
IN: 'in';
INDIRECT: 'indirect';
INHERITS: 'inherits';
INTERFACE: 'interface';
ISOLATED: 'isolated';
KIND: 'kind';
LAYOUT: 'layout';
LIFETIME: 'lifetime';
MODULE: 'module';
MACRO: 'macro';
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
OCT_LITERAL:        '0' [o] '_'* [0-7] ([0-7_]* [0-7])?;
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
WS:                 [ \t\u000C]+ -> channel(HIDDEN);
COMMENT:            '/*' .*? '*/'    -> channel(HIDDEN);
LINE_COMMENT:       '//' ~[\r\n]*    -> channel(HIDDEN);

UNDERSCORE: '_'+;
IDENTIFIER
    : '_'+ [A-Za-z0-9] [A-Za-z0-9_]*
    | [A-Za-z] [A-Za-z0-9_]*
    ;

// This is not fully correct, that should not be hidden, 
// but we don't want to handle correctly NEW_LINE in this grammar
NEW_LINE: ([\r][\n]|[\n]|[\r])  -> channel(HIDDEN);

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
LESS_THAN_EQUAL: '<=';
GREATER_THAN_EQUAL: '>=';
LESS_THAN_GREATER_THAN: '<>';
EQUAL_GREATER_THAN: '=>';
MINUS_GREATER_THAN: '->';
VERTICAL_BAR_GREATER_THAN: '|>';
DOUBLE_LESS_THAN_EQUAL: '<<=';
DOUBLE_GREATER_THAN_EQUAL: '>>=';

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