This library is a modification of the original @angular/elements library, to reuse the ComponentNgElementStrategy class in custom StrategyFactory implementations.

We must to do this because the original package (@angular/elements) doesn't have a reference to the ComponentNgElementStrategy that allow reuse it in custom implementations.
