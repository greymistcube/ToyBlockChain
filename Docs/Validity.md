# Validity and Soundness

## Definitions

In order to be more concise, we differentiate validity and
soundness of `Block`s and `Transaction` in a following way.
Validity refers to relational compatibility with the `Node`
and soundness refers to structural integrity. In other words,
a `Block` or a `Transaction` is valid with respect to a `Node`
if it can be accepted by said `Node` and is sound if it has
the required internal consistency on its own.

Although there are other entities such as `BlockHeader`s and
`Operation`s which constitue parts of `Block`s, we mainly
focus on `Block`s and `Transaction`s.

The list below is *not exhaustive*. Some depend on implementation
and/or choice.

### `Block` Validity

A `Block` is valid with respect to a `Node` if it can
be appended to the `BlockChain`.

* Its `BlockHeader` is valid.
* Its `Transaction` is valid.

### `Block` Soundness

A `Block` is sound if its data is consistent.

* The `TransactionHashString` of its `BlockHeader` and
  the `HashString` of `Transaction` are the same.
* Its `BlockHeader` is sound.
* Its `Transaction` is sound.

### `Transaction` Validity

A `Transaction` is valid with respect to a `Node` if it can be
consumed by the `AccountCatalogue`.

* The `Count` is equal to the `Count + 1` of the corresponding
  `Sender`'s account.
* Its `Operation` is valid.

### `Transaction` Soundness

A `Transaction` is sound if its data is consistent.

* Its `Sender` is verified via `PublicKey`.
* Its `Signature` is also verified via `PublicKey`.
* Its `Operation` is sound.
