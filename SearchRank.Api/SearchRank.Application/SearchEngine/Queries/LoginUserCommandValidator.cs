using FluentValidation;

namespace SearchRank.Application.SearchEngine.Queries
{
    internal class SearchEngineQueryValidator : AbstractValidator<SearchEngineQuery>
    {
        public SearchEngineQueryValidator()
        {
            RuleFor(x => x.Keyword)
                .NotEmpty()
                .WithMessage(x => $"{nameof(x.Keyword)} is required.");

            RuleFor(x => x.TargetUrl)
                .NotEmpty()
                .WithMessage(x => $"{nameof(x.TargetUrl)} is required.");
        }
    }
}
