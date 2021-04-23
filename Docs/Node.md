# `Node`

## `Node` Components

A `Node` has three main components.

* `BlockChain`: The history of all transactions.
* `TransactionPool`: A pool of pending transactions.
* `AccountCatalogue`: The current state.

## `Node` Machine

One interpretation of a `Node` may be is to consider it as a machine.
As an entity, when a `Node` takes a unit of data, the following happens.

* Record it to the `BlockChain` (non-volatile memory).
* Put it in to the `TransactionPool` (volatile memory).
* Compute and output to the `AccountCatalogue` (output device).

## `Node` Inputs

A `Node` is capable of taking two kinds of inputs, `Block`s and `Transaction`s.

## `Node` Integrity

We define a `Node` to be integral if all `Block`s in its `BlockChain` and
`Transaction`s in its `TransactionPool` are valid. One way to ensure such
integrity is to only accept valid `Block`s and `Transaction`s to
enter the system.

A problem arises when two or more conflicting `Transaction`s, where consumption
of one would result in invalidating another, enter the system, each as
a valid `Transaction` on its own. This is not particularly desireable.
We discuss several possible approaches to resolve this problem below.

### `TransactionPool` Purge

One way to resolve this problem would be to purge all pending `Transaction`s
in the `TransactionPool` that have become invalid as a result of a `Node`'s
state transition. Note that this keeps the system to be integral in accordance
to the definition above.

### `Transaction` Compatability Test

One way to avoid this problem would be is to avoid accepting any `Transaction`
that would conflict with a `Transaction` already in the `TransactionPool`,
that is to accept only compatible `Transaction`s. Although this also
adheres to keeping the system integral, there are a couple of issues
to this approach.

Firstly, implementing a function for checking whether two `Transaction`s are
compatible may be a difficult task. For one, designing a proper function
to check the compatibility between two `Transaction`s without performing any
state transition my even be impossible. Another way is to compute the
results of all possible state transitions of all `Transactions` and see
if a new `Transaction` would be valid against all states, but this would
require a significant amount of computational resource of the `Node`.
Yet another method would be to index all would be affected `Account`s
by pending `Transaction`s and do a simpler check, but this would add
a significant amount of complexity to the already quite complex system.

Secondly, due to distributive nature of the network, an incompatible
`Transaction` may enter the `BlockChain` from another `Node`. If the `Node`
were to accept such incoming `Block`, this would require either
emptying the `TransactionPool` or purging all incompatible `Transaction`s.
For this point alone, there seems to be little to no advantage over
the first approach.

### Invalid `Transaction` Mining

As a policy matter, we may only require `Transaction`s to be
valid at the point of entry, and ignore whether they become invalid or not
during and after a state transition. An invalid `Transaction` can still
be mined and added to the ledger, but would simply result in no state change.
Although this no longer enforces `Node` integrity according to our definition,
it is the easiest to implement.

There are two main issues with this approach. One, depending on how validity
of a `Transaction` is defined, `TransactionPool` can get overly cluttered,
making it prone to a `Transaction` flooding attack. Two, significant amount
of mining resource is wasted on mining invalid `Transaction`s.
