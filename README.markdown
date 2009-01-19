dotGit
======

STATE:  pre-alpha
dotGit is in start-up phase

Go to the [Wiki for more info](http://github.com/pheew/dotgit/wikis) 


The unit tests require a submodule. Be sure to fetch it with 'git submodule update --init'
If it's your first time looking at the source have a look at the class diagram in the dotGit project. It should get you up to speed.


Bugs / Features
===============

If you find any bugs or would like to see a feature implemented please report it at the [Lighthouse tracker](http://pheew.lighthouseapp.com/projects/21305/home)


How about Mono?
===============

While it would be nice to be able to use dotGit on Mono it is NOT supported at the moment. The main reason behind this is the fact that we're using native Windows API calls to create memory maps because this is not supported by the .NET runtime. Using memory maps drastically improves dotGit's performance and we're not willing to sacrifice this huge performance boost for portability. 
Furthermore Mono is, for the most part, used on \*NIX systems. Obviously, Git support on these OS's is excellent. Using regular Git is the sane choice here.

That being said, we might add support for Mono in the future by providing builds for Mono where memory maps are created using the Mono libraries. (Making these builds \*NIX only)