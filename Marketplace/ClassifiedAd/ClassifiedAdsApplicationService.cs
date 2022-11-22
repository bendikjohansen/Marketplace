using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.Shared;
using Marketplace.Framework;
using static Marketplace.ClassifiedAd.Contracts;

namespace Marketplace.ClassifiedAd;

public class ClassifiedAdsApplicationService
{
    private readonly IClassifiedAdRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrencyLookup _currencyLookup;

    public ClassifiedAdsApplicationService(IClassifiedAdRepository repository, ICurrencyLookup currencyLookup,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currencyLookup = currencyLookup;
        _unitOfWork = unitOfWork;
    }

    public Task Handle(object command) => command switch
    {
        V1.Create cmd => HandleCreate(cmd),
        V1.SetTitle cmd => HandleUpdate(cmd.Id,
            ad => ad.SetTitle(ClassifiedAdTitle.FromString(cmd.Title))),
        V1.UpdateText cmd => HandleUpdate(cmd.Id,
            ad => ad.UpdateText(ClassifiedAdText.FromString(cmd.Text))),
        V1.UpdatePrice cmd => HandleUpdate(cmd.Id,
            ad => ad.UpdatePrice(Price.FromDecimal(cmd.Amount, cmd.CurrencyCode, _currencyLookup))),
        V1.AddPicture cmd => HandleUpdate(cmd.Id, ad => ad.AddPicture(cmd.Url, new PictureSize(cmd.Width, cmd.Height))),
        V1.RequestToPublish cmd => HandleUpdate(cmd.Id, ad => ad.RequestToPublish()),
        V1.ApprovePublish cmd => HandleUpdate(cmd.Id, ad => ad.Publish(new UserId(cmd.UserId))),
        _ => Task.CompletedTask
    };

    private async Task HandleCreate(V1.Create cmd)
    {
        if (await _repository.Exists(new ClassifiedAdId(cmd.Id)))
        {
            throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");
        }

        var classifiedAd = new Domain.ClassifiedAd.ClassifiedAd(
            new ClassifiedAdId(cmd.Id),
            new UserId(cmd.OwnerId));

        await _repository.Add(classifiedAd);
        await _unitOfWork.Commit();
    }

    private async Task HandleUpdate(Guid id, Action<Domain.ClassifiedAd.ClassifiedAd> operation)
    {
        var classifiedAd = await _repository.Load(new ClassifiedAdId(id));
        if (classifiedAd == null)
        {
            throw new InvalidOperationException($"Entity with id {id} cannot be found");
        }

        operation(classifiedAd);
        await _unitOfWork.Commit();
    }
}
