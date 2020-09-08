using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Contracts.SpellChecker;
using SpellChecker.Engines.cSpell.NlpUtil;

namespace SpellChecker.Controllers {
	[ApiController]
	[Route("api/[controller]")]
	public class CorrectController : ControllerBase {
		private readonly ILogger<CorrectController> _logger;
		private readonly ISpellChecker _checker;

		public CorrectController(ILogger<CorrectController> logger, ISpellChecker checker) {
			_logger = logger;
			_checker = checker;
		}

		[HttpGet]
		public async Task<ActionResult<String>> Get() {
			return _checker.Correct("CSpell corrects speling errorrs, mer ge andsplit for along time");
		}

		[HttpPost]
		public async Task<ActionResult<List<OutputTextDTO>>> Correct(List<InputTextDTO> inputList) {
			return await ProcessList(inputList);
		}

		private async Task<List<OutputTextDTO>> ProcessList(List<InputTextDTO> inputList) {
			var ret = new ConcurrentBag<OutputTextDTO>();

			Parallel.ForEach(inputList, input => {
				var item = new OutputTextDTO() {
					Text = input.Text,
						ResultText = input.Text,
						ResultTokens = new List<Shared.Contracts.SpellChecker.TokenObj>()
				};

				var watch = System.Diagnostics.Stopwatch.StartNew();
				try {
					var result = _checker.CorrectExt(input.Text);
					item.ResultText = result.Item1;
					if (result.Item2 != null)
						foreach (var token in result.Item2) {
							item.ResultTokens.Add(new Shared.Contracts.SpellChecker.TokenObj {
								Token = token.GetOrgTokenStr(),
									ResultToken = token.GetTokenStr()
							});

						}

				} catch (Exception ex) {
					_logger.LogError($"Unhandled exception: {ex.Message}, engine re-initialization started");
					_checker.Recover();
					_logger.LogError($"Engine re-initialization has completed, no correction has been done");
				}
				watch.Stop();
				_logger.LogInformation($"[{watch.ElapsedMilliseconds}ms] Corrected [{input.Text}] -> [{item.ResultText}]");
				ret.Add(item);
			});
			return ret.ToList();
		}
	}
}